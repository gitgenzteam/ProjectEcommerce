using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlTypes;
using System.Data.Common;

namespace Ecommerce.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        string connectionString = "Server=localhost;Database=ecommerce;User=root;Password=12345678@Abc;";

        //public IDbConnection getConnection()
        //{
        //    IDbConnection dbConnection = new MySqlConnection(connectionString);
        //    return dbConnection;
        //}

        [HttpGet("GetUser/{id_user}")]
        public IActionResult GetUser([FromRoute] string id_user)
        {
            IDbConnection dbConnection = new MySqlConnection(connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("id", id_user);

            var user = dbConnection.Query<UserEntity>($"SELECT * FROM user where user_id = @id", parameters);

            return StatusCode(200, user);
        }
        [HttpGet("Login/{user_name}/{password}")] 
        public IActionResult Login([FromRoute] string user_name, string password)
        {
            IDbConnection dbConnection = new MySqlConnection(connectionString);
            var parametes = new DynamicParameters();
            parametes.Add("username", user_name);
            parametes.Add("Password", password);
            var user = dbConnection.Query<UserEntity>($"SELECT * FROM user where user_name=@username and password=@Password;", parametes);
            return StatusCode(200, user);
        }
        [HttpDelete("DeleteUser/{user_id}")]
        public IActionResult DeleteUser([FromRoute] Guid user_id)
        {
            IDbConnection dbConnection = new MySqlConnection(connectionString);

            var parametes = new DynamicParameters();
            parametes.Add("userid", user_id);
            int user = dbConnection.Execute($"DELETE FROM user where user_id=@userid", parametes);
            if (user > 0)
                return StatusCode(200, true);
            else
                return StatusCode(404, false);
        }
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] UserEntity newUser)
        {
            IDbConnection dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            try
            {
                Guid user_id = Guid.NewGuid();
                string userIdString = user_id.ToString();
                var parametes = new DynamicParameters();
                parametes.Add("in_username", newUser.user_name);
                parametes.Add("in_password", newUser.password);
                parametes.Add("in_dateofbirth", newUser.date_of_birth);
                parametes.Add("userIdString", user_id);


                int user = dbConnection.Execute($"INSERT INTO user(user_id, user_name, password, date_of_birth) VALUE(@userIdString, @in_username, @in_password, @in_dateofbirth) ", parametes);

                if (user > 0)
                    return StatusCode(200, true);
                else
                    return StatusCode(404, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, false);
            }
            finally
            {
                dbConnection.Close();
            }
        }
        [HttpPut("UpdateUser/{user_id}/{user_name}/{password}/{date_of_birth}")]
        public IActionResult UpdateUser([FromRoute] Guid user_id, string user_name, string password, DateTime date_of_birth)
        {

            IDbConnection dbConnection = new MySqlConnection(connectionString);
            var parametes = new DynamicParameters();
            parametes.Add("in_username", user_name);
            parametes.Add("in_password", password);
            parametes.Add("in_dateofbirth", date_of_birth);
            parametes.Add("in_userid", user_id);

            if (user_id == Guid.Empty)
            {
                return NotFound($"No user is founded with Id={user_id}");
            }
            else
            {
                int user = dbConnection.Execute($"UPDATE user SET user_name = @in_username, password = @in_password, date_of_birth=@in_dateofbirth  WHERE user_id = @in_userid ", parametes);
                return StatusCode(200);
            }
        }
    }
}
