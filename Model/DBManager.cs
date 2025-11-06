using Newtonsoft.Json;
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

        public string SelectCarClaimByID(int claimID) 
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


        public void InsertNewCarClaim(VehicleClaim claimData)
        {;

            SQLiteCommand clientCommand = new SQLiteCommand(@"
                INSERT INTO clients (dni, name, surname, phone_number, email)
                VALUES (@dni, @name, @surname, @phone, @mail)
            ");
            clientCommand.Parameters.AddWithValue("@dni", claimData.ClientDNI);
            clientCommand.Parameters.AddWithValue("@name", claimData.ClientName);
            clientCommand.Parameters.AddWithValue("@surname", claimData.ClientSurname);
            clientCommand.Parameters.AddWithValue("@phone", claimData.PhoneNumber);
            clientCommand.Parameters.AddWithValue("@mail", claimData.Email);

            SQLiteCommand vehicleCommand = new SQLiteCommand( @"
                INSERT INTO vehicles (brand, model, license_plate, registered_owner)
                VALUES (@brand, @model, @plate, @owner)
            ");
            vehicleCommand.Parameters.AddWithValue("@brand", claimData.vehicleBrand);
            vehicleCommand.Parameters.AddWithValue("@model", claimData.vehicleModel);
            vehicleCommand.Parameters.AddWithValue("@plate", claimData.licensePlate);
            vehicleCommand.Parameters.AddWithValue("@owner", claimData.registeredOwner);

            SQLiteCommand policyCommand = new SQLiteCommand(@"
                INSERT INTO policies (policy_number, company, coverage)
                VALUES (@policy, @company, @coverage)
            ");
            policyCommand.Parameters.AddWithValue("@policy", claimData.PolicyNumber);
            policyCommand.Parameters.AddWithValue("@company", claimData.CompanyName);
            policyCommand.Parameters.AddWithValue("@coverage", claimData.Coverage);

            SQLiteCommand fullCommand = new SQLiteCommand(@"
                WITH 
                new_client AS ( SELECT client_id FROM clients WHERE clients.dni = @clientDNI),
                new_vehicle AS ( SELECT vehicle_id FROM vehicles WHERE vehicles.license_plate = @carPlate),
                new_policy AS (SELECT policy_id FROM policies WHERE policies.policy_number = @policy)
                
                INSERT INTO claims (claim_number, description, direction, city, date_and_hour, policy_id, vehicle_id, client_id)
                SELECT @claim, @description, @direction, @city, @dateAndHour, 
                new_policy.policy_id, new_vehicle.vehicle_id, new_client.client_id
                FROM new_policy, new_vehicle, new_client;    
            ");
            fullCommand.Parameters.AddWithValue("@clientDNI", claimData.ClientDNI);
            fullCommand.Parameters.AddWithValue("@carPlate", claimData.licensePlate);
            fullCommand.Parameters.AddWithValue("@policy", claimData.PolicyNumber);
            fullCommand.Parameters.AddWithValue("@claim", claimData.ClaimNumber);
            fullCommand.Parameters.AddWithValue("@description", claimData.Description);
            fullCommand.Parameters.AddWithValue("@direction", claimData.Direction);
            fullCommand.Parameters.AddWithValue("@city", claimData.City);
            fullCommand.Parameters.AddWithValue("@dateAndHour", claimData.DateAndHour);

            InsertQuery(clientCommand);
            InsertQuery(vehicleCommand);
            InsertQuery(policyCommand);
            InsertQuery(fullCommand);
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
                //exception
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
                //exception
            }
            sqlite.Close();
        }
    }
}
