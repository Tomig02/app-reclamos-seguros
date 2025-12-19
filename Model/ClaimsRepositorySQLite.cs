using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SQLite;

namespace app_reclamos_seguros.Model
{
    /// <summary>
    /// Middleware handling the sql-querying and interactions with the database
    /// </summary>
    public class ClaimsRepositorySQLite : IClaimsRepository
    {
        private SQLiteConnection sqlite;

        /// <summary>
        /// Creates a dbmanager object, creating and storing a connection with the database
        /// </summary>
        public ClaimsRepositorySQLite()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Public\SQLite.db");
            string absolutePath = Path.GetFullPath(dbPath);

            if (File.Exists(absolutePath))
            {
                sqlite = new SQLiteConnection($"Data Source={absolutePath}");
            }
            else
            {
                SQLiteConnection.CreateFile(absolutePath);
                sqlite = new SQLiteConnection($"Data Source={absolutePath}");

                SQLiteCommand sqlSetupCommand = new SQLiteCommand($@"
                    CREATE TABLE clients (client_id INTEGER PRIMARY KEY AUTOINCREMENT , dni INTEGER, name TEXT, surname TEXT, phone_number INTEGER, email TEXT);
                    CREATE TABLE claim_entries (entry_id INTEGER PRIMARY KEY AUTOINCREMENT , claim_id INTEGER, comment TEXT, date_and_time DATETIME,
	                    FOREIGN KEY (claim_id) REFERENCES claims(claim_id)                     
                    );
                    CREATE TABLE vehicles (vehicle_id INTEGER PRIMARY KEY AUTOINCREMENT , brand TEXT, model TEXT, license_plate TEXT, registered_owner TEXT);
                    CREATE TABLE policies (policy_id INTEGER PRIMARY KEY AUTOINCREMENT, policy_number INTEGER, company TEXT, coverage TEXT);    
            
                    CREATE TABLE claims (claim_id INTEGER PRIMARY KEY AUTOINCREMENT, claim_number INTEGER, description TEXT, direction TEXT, city TEXT, date_and_hour DATETIME, 
	                    policy_id INTEGER, vehicle_id INTEGER, client_id INTEGER, archived BOOLEAN,
    
	                    FOREIGN KEY (policy_id) REFERENCES policies(policy_id),
                        FOREIGN KEY (vehicle_id) REFERENCES vehicles(vehicle_id), 
                        FOREIGN KEY (client_id) REFERENCES clients(client_id)
                    );
                ");
                sqlSetupCommand.Connection = sqlite;

                sqlite.Open();
                sqlSetupCommand.ExecuteNonQuery();
                sqlite.Close();
            }
        }


        /// <summary>
        /// receives a DataTable object and serializes it into a JSON string
        /// </summary>
        /// <param name="data"> Datable object holding the data </param>
        /// <returns> A JSON formated string </returns>
        private string ToJsonString(DataTable data) 
        {
            return JsonConvert.SerializeObject(data); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claimNum"> The identifying claim number </param>
        /// <param name="shouldBeArchived"> Truth value of if should be archived </param>
        /// <exception cref="DatabaseException"> The claim to be archived doesn't exist </exception>
        public void SetArchived(int claimNum, bool shouldBeArchived) 
        {
            if (RecordExists("claims", "claim_number", claimNum))
            {
                SQLiteCommand updateCommand = CreateCommand(
                    "UPDATE claims SET archived = @value WHERE claims.claim_number = @claim",
                    ("@claim", claimNum),
                    ("@value", shouldBeArchived)
                );

                InsertQuery(updateCommand);
            }
            else throw new DatabaseException($"The claim number is not valid", new InvalidOperationException());
        }

        /// <summary>
        /// Returns a reduced set of the data of a stored claim, meant for identifying a claim
        /// </summary>
        /// <returns> 
        /// JSON formated string: 
        /// [ {"claim_number": int, "date_and_hour": string(UTC−03:00), "name": string, "surname": string}, ... ]
        /// </returns>
        public string GetActiveClaimsList()
        {
            SQLiteCommand selectCommand = CreateCommand($@"
                SELECT claims.claim_number, claims.date_and_hour, claims.archived, clients.name, clients.surname FROM claims JOIN clients
                WHERE claims.client_id = clients.client_id AND claims.archived = false
            ");

            return SelectQuery(selectCommand);
        }

        /// <summary>
        /// Select all car claims, filtered by if it's archived
        /// </summary>
        /// <returns> A list of filtered claims </returns>
        /// 
        public string GetArchivedClaimsList()
        {
            SQLiteCommand selectCommand = CreateCommand($@"
                SELECT claims.claim_number, claims.date_and_hour, claims.archived, clients.name, clients.surname FROM claims JOIN clients
                WHERE claims.client_id = clients.client_id AND claims.archived = true
            ");

            return SelectQuery(selectCommand);
        }

        /// <summary>
        /// Returns the complete data of one specified claim, selected from the database using the claim number
        /// </summary>
        /// <param name="claimNumber"> The claim number asigned by the insurance company </param>
        /// <returns>
        /// JSON formated string:
        /// [ { ...claimTableRow, ...clientsTableRow, ...vehiclesTableRow, ...policiesTableRow} ]
        /// </returns>
        public string GetByID(int claimNumber) 
        {
            SQLiteCommand selectCommand = CreateCommand( @"
                SELECT * FROM claims JOIN clients, vehicles, policies 
                WHERE claims.claim_number = @claimID
                AND claims.client_id = clients.client_id 
                AND claims.vehicle_id = vehicles.vehicle_id 
                AND claims.policy_id = policies.policy_id
            ",
                ("@claimID", claimNumber)
            );
            
            return SelectQuery(selectCommand);
        }

        /// <summary>
        /// Inserts the complete data of a claim into the database
        /// </summary>
        /// <param name="claimData"> the object containing the claim's data </param>
        /// <exception cref="DatabaseException"> A claim with the same claim number already exists in the database </exception>
        public void SetNewClaim(VehicleClaim claimData)
        {
            ;
            bool claimExists = RecordExists("claims", "claim_number", claimData.ClaimNumber);

            if (claimExists)
            {
                throw new DatabaseException($"The claim {claimData.ClaimNumber} already exists in the database", new InvalidOperationException());
            }
            else
            {
                bool clientExists = RecordExists("clients", "dni", claimData.ClientDNI);
                bool vehicleExists = RecordExists("vehicles", "license_plate", claimData.licensePlate);
                bool policyExists = RecordExists("policies", "policy_number", claimData.PolicyNumber);

                if (!clientExists)
                {
                    SQLiteCommand clientCommand = CreateCommand(
                        "INSERT INTO clients (dni, name, surname, phone_number, email) VALUES (@dni, @name, @surname, @phone, @mail)",
                        ("@dni", claimData.ClientDNI),
                        ("@name", claimData.ClientName),
                        ("@surname", claimData.ClientSurname),
                        ("@phone", claimData.PhoneNumber),
                        ("@mail", claimData.Email)
                    );
                    InsertQuery(clientCommand);
                }

                if (!vehicleExists)
                {
                    SQLiteCommand vehicleCommand = CreateCommand(
                        "INSERT INTO vehicles (brand, model, license_plate, registered_owner) VALUES (@brand, @model, @plate, @owner)",
                        ("@brand", claimData.vehicleBrand),
                        ("@model", claimData.vehicleModel),
                        ("@plate", claimData.licensePlate),
                        ("@owner", claimData.registeredOwner)
                    );
                    InsertQuery(vehicleCommand);
                }

                if (!policyExists)
                {
                    SQLiteCommand policyCommand = CreateCommand(
                        "INSERT INTO policies (policy_number, company, coverage) VALUES (@policy, @company, @coverage)",
                        ("@policy", claimData.PolicyNumber),
                        ("@company", claimData.CompanyName),
                        ("@coverage", claimData.Coverage)
                    );

                    InsertQuery(policyCommand);
                }

                var fullCommand = CreateCommand(
                    @"
                    WITH 
                        new_client AS (SELECT client_id FROM clients WHERE dni = @clientDNI),
                        new_vehicle AS (SELECT vehicle_id FROM vehicles WHERE license_plate = @carPlate),
                        new_policy AS (SELECT policy_id FROM policies WHERE policy_number = @policy)
                    
                    INSERT INTO claims (claim_number, description, direction, city, date_and_hour, archived, policy_id, vehicle_id, client_id)
                    SELECT @claim, @description, @direction, @city, @dateAndHour, @archived,
                           new_policy.policy_id, new_vehicle.vehicle_id, new_client.client_id
                    FROM new_policy, new_vehicle, new_client
                    ",
                    ("@clientDNI", claimData.ClientDNI),
                    ("@carPlate", claimData.licensePlate),
                    ("@policy", claimData.PolicyNumber),
                    ("@claim", claimData.ClaimNumber),
                    ("@description", claimData.Description),
                    ("@direction", claimData.Direction),
                    ("@city", claimData.City),
                    ("@dateAndHour", claimData.DateAndHour),
                    ("@archived", false)
                );

                InsertQuery(fullCommand);
            }
        }

        /// <summary>
        /// Returns a list of all the entries of a specified claim, identifyed by the claim number
        /// </summary>
        /// <param name="claimNumber"> The claim number asigned by the insurance company </param>
        /// <returns>
        /// JSON formated string:
        /// [ {"entry_id": int, "claim_id": int, "comment": string, "date_and_time": string(UTC−03:00)} ]
        /// </returns>
        /// <exception cref="DatabaseException"></exception>
        public string GetAllReportsByID(int claimNumber) 
        {
            // identify the claim's database id, for later use in the search for the entries
            int claimID = SelectFromEntityGetID("claims", "claim_id", "claim_number", claimNumber);

            // if the claim exists in the database
            if (claimID != -1)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(@"
                    SELECT * FROM claim_entries WHERE claim_entries.claim_id = @claim
                ");
                selectCommand.Parameters.AddWithValue("@claim", claimID);

                return SelectQuery(selectCommand);
            }
            else
            {
                throw new DatabaseException($"The claim {claimNumber} couldn't be found", new InvalidOperationException());
            }
        }

        /// <summary>
        /// Insert a new comment entry in the database for a specific claim 
        /// </summary>
        /// <param name="newReport"> The new entry object to be saved </param>
        /// <exception cref="DatabaseException"> The database doesn't have a claim with the specified number </exception>
        public void SetNewReport(ClaimReportEntry newReport)
        {
            // identify the claim's database id, for later use in the search for the entries
            int claimID = SelectFromEntityGetID("claims", "claim_id", "claim_number", newReport.ClaimNumber);
            bool isArchived = IsArchived(newReport.ClaimNumber);
            
            if (claimID == -1)
                throw new DatabaseException($"The claim {newReport.ClaimNumber} couldn't be found", new InvalidOperationException());
            if (isArchived)
                throw new DatabaseException($"The claim {newReport.ClaimNumber} is archived and cant receive new entries", new InvalidOperationException());

            SQLiteCommand reportCommand = CreateCommand(
                    "INSERT INTO claim_entries (claim_id, comment, date_and_time) VALUES (@claim, @comment, @datetime)",
                    ("@claim", claimID),
                    ("@comment", newReport.Comment),
                    ("@datetime", newReport.DateAndTime)
                );
            InsertQuery(reportCommand);
        }

        /// <summary>
        /// Creates a SQLite Command
        /// </summary>
        /// <param name="query"> The full query string </param>
        /// <param name="parameters"> The parameters to be added into the query </param>
        /// <returns> the created SQLiteCommand object </returns>
        private SQLiteCommand CreateCommand(string query, params (string param, object value)[] parameters)
        {
            var command = new SQLiteCommand(query, sqlite);
            foreach (var (param, value) in parameters)
            {
                command.Parameters.AddWithValue(param, value);
            }
            return command;
        }

        /// <summary>
        /// Checks if a table contains a row with the specified value
        /// </summary>
        /// <param name="table"> The table to search in </param>
        /// <param name="column"> The column that holds the searched data</param>
        /// <param name="value"> The value to match </param>
        /// <returns> true if the value exists in any row, false if it doesn't </returns>
        /// <exception cref="DatabaseException"> The database couldn't run the query, contains an SQLiteException </exception>
        private bool RecordExists(string table, string column, object value)
        {
            var command = new SQLiteCommand($"SELECT EXISTS(SELECT 1 FROM {table} WHERE {column} = @value)", sqlite);
            command.Parameters.AddWithValue("@value", value);

            try
            {
                sqlite.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                sqlite.Close();
                return count == 1;
            }
            catch(SQLiteException ex)
            {
                throw new DatabaseException($"There was an error executing query: {command.CommandText}", ex);
            }
            
        }

        /// <summary>
        /// Handles the execution of a SELECT query
        /// </summary>
        /// <param name="cmd"> the command object with the query to be executed </param>
        /// <returns> a JSON formated string with the array of selected rows </returns>
        /// <exception cref="DatabaseException"> The database couldn't run the query, contains an SQLiteException </exception>
        private string SelectQuery(SQLiteCommand cmd) 
        {
            SQLiteDataAdapter ad;
            DataTable dt = new DataTable();
            cmd.Connection = sqlite;

            try
            {
                sqlite.Open();
                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt);
            }
            catch (SQLiteException ex)
            {
                throw new DatabaseException($"There was an error executing query: {cmd.CommandText}", ex);
            }

            sqlite.Close();
            return ToJsonString(dt);
        }
        
        /// <summary>
        /// Handles the execution of a INSERT instruction
        /// </summary>
        /// <param name="cmd"> the command object with the query to be executed </param>
        /// <exception cref="DatabaseException"> The database couldn't run the query, contains an SQLiteException </exception>
        private void InsertQuery(SQLiteCommand cmd) 
        {
            cmd.Connection = sqlite;

            try
            {
                sqlite.Open();
                cmd.ExecuteNonQuery();
            }
            catch(SQLiteException ex)
            {
                throw new DatabaseException($"There was an error executing query: {cmd.CommandText}", ex);
            }
            sqlite.Close();
        }

        /// <summary>
        /// Look for the database id of a specified table's row
        /// </summary>
        /// <param name="table"> The table's name as it exists in the database </param>
        /// <param name="idColumn"> The column's name for the SELECT instruction</param>
        /// <param name="whereColumn"> the column name for the WHERE instruction</param>
        /// <param name="whereValue"> the desired value for the WHERE column </param>
        /// <returns> the int value of the id, being -1 if not found </returns>
        /// <exception cref="InvalidOperationException"> The args didn't get a result from the database (wrong args or no rows inserted with the value) </exception>
        private int SelectFromEntityGetID(string table, string selectColumn, string whereColumn, object whereValue)
        {
            var command = CreateCommand(
                $"SELECT {selectColumn} FROM {table} WHERE {whereColumn} = @value",
                ("@value", whereValue)
            );

            try
            {
                var result = JArray.Parse(SelectQuery(command));

                if (result.Count == 0)
                    return -1;
                return result[0][selectColumn]!.Value<int>();
            }
            catch (DatabaseException ex) { throw; }
        }

        /// <summary>
        /// Checks if the claim is archived or not
        /// </summary>
        /// <param name="claimNum"> the number of the claim assigned by insurance </param>
        /// <returns> If it's archived or not </returns>
        /// <exception cref="DatabaseException"> There is an error in the query </exception>
        private bool IsArchived(int claimNum)
        {
            SQLiteCommand command = CreateCommand(@"
                SELECT EXISTS(SELECT 1 FROM claims WHERE archived = true AND claim_number = @claimNum)
            ",
                ("@claimNum", claimNum)
            );

            try
            {
                sqlite.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                sqlite.Close();
                return count == 1;
            }
            catch (SQLiteException ex)
            {
                throw new DatabaseException($"There was an error executing query: {command.CommandText}", ex);
            }
        }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception innerException) : base(message, innerException) {}
    }
}
