using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SQLite;

namespace app_reclamos_seguros.Model
{
    public class DBManager
    {
        private SQLiteConnection sqlite;

        public DBManager()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Public\SQLite.db");
            sqlite = new SQLiteConnection($"Data Source={Path.GetFullPath(dbPath)}");
        }

        private string ToJsonString(DataTable data) 
        {
            return JsonConvert.SerializeObject(data); ;
        }

        public string SelectListAllCarClaims()
        {
            SQLiteCommand selectCommand = new SQLiteCommand($@"
                SELECT claims.claim_number, claims.date_and_hour, clients.name, clients.surname FROM claims JOIN clients
                WHERE claims.client_id = clients.client_id
            ");

            return SelectQuery(selectCommand);
        }

        public string SelectCarClaimByNumber(int claimID) 
        {
            SQLiteCommand selectCommand = new SQLiteCommand( @"
                SELECT * FROM claims JOIN clients, vehicles, policies 
                WHERE claims.claim_number = @claimID
                AND claims.client_id = clients.client_id 
                AND claims.vehicle_id = vehicles.vehicle_id 
                AND claims.policy_id = policies.policy_id
            ");
            selectCommand.Parameters.AddWithValue("@claimID", claimID);
            
            return SelectQuery(selectCommand);
        }

        public string SelectClaimEntries(int claimNumber) 
        {
            SQLiteCommand selectClaimID = new SQLiteCommand(@"
                SELECT claim_id FROM claims WHERE claim_number = @claim 
            ");
            selectClaimID.Parameters.AddWithValue("@claim", claimNumber);

            int claimID;
            var resultArray = JArray.Parse(SelectQuery(selectClaimID));

            if (resultArray.Count > 0)
            {
                claimID = (int)resultArray[0]["claim_id"]!;

                SQLiteCommand selectCommand = new SQLiteCommand(@"
                    SELECT * FROM claim_entries WHERE claim_entries.claim_id = @claim
                ");
                selectCommand.Parameters.AddWithValue("@claim", claimID);

                return SelectQuery(selectCommand);
            }
            else
            {
                throw new DatabaseException($"The claim {claimNumber} doesn't exist", new InvalidOperationException());
            }
        }

        public void InsertNewClaimReportEntry(ClaimReportEntry newReport)
        {
            SQLiteCommand selectClaimID = new SQLiteCommand(@"
                SELECT claim_id FROM claims WHERE claim_number = @claim 
            ");
            selectClaimID.Parameters.AddWithValue("@claim", newReport.ClaimNumber);

            int claimID;
            var resultArray = JArray.Parse(SelectQuery(selectClaimID));

            if (resultArray.Count > 0)
            {
                claimID = (int)resultArray[0]["claim_id"]!;

                SQLiteCommand reportCommand = CreateInsertCommand(
                    "INSERT INTO claim_entries (claim_id, comment, date_and_time) VALUES (@claim, @comment, @datetime)",
                    ("@claim", claimID),
                    ("@comment", newReport.Comment),
                    ("@datetime", newReport.DateAndTime)
                );
                InsertQuery(reportCommand);
            }
            else
            {
                throw new DatabaseException($"The claim {newReport.ClaimNumber} doesn't exist", new InvalidOperationException());
            }
        }

        public void InsertNewCarClaim(VehicleClaim claimData)
        {;
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
                    SQLiteCommand clientCommand = CreateInsertCommand(
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
                    SQLiteCommand vehicleCommand = CreateInsertCommand(
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
                    SQLiteCommand policyCommand = CreateInsertCommand(
                        "INSERT INTO policies (policy_number, company, coverage) VALUES (@policy, @company, @coverage)",
                        ("@policy", claimData.PolicyNumber),
                        ("@company", claimData.CompanyName),
                        ("@coverage", claimData.Coverage)
                    );

                    InsertQuery(policyCommand);
                }

                var fullCommand = CreateInsertCommand(
                    @"
                    WITH 
                        new_client AS (SELECT client_id FROM clients WHERE dni = @clientDNI),
                        new_vehicle AS (SELECT vehicle_id FROM vehicles WHERE license_plate = @carPlate),
                        new_policy AS (SELECT policy_id FROM policies WHERE policy_number = @policy)
                    
                    INSERT INTO claims (claim_number, description, direction, city, date_and_hour, policy_id, vehicle_id, client_id)
                    SELECT @claim, @description, @direction, @city, @dateAndHour, 
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
                    ("@dateAndHour", claimData.DateAndHour)
                );

                InsertQuery(fullCommand);
            }
        }

        private SQLiteCommand CreateInsertCommand(string query, params (string param, object value)[] parameters)
        {
            var command = new SQLiteCommand(query, sqlite);
            foreach (var (param, value) in parameters)
            {
                command.Parameters.AddWithValue(param, value);
            }
            return command;
        }
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

        public string SelectQuery(SQLiteCommand cmd) 
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
        
        public void InsertQuery(SQLiteCommand cmd) {
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
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception innerException) : base(message, innerException) 
        {

        }
    }
}
