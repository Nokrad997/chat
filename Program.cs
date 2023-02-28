using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MySqlConnector;

/*
    To Do:
        - add admin mode (add, delete, edit users, groups, messages)
        - add chat (chat is only available for friends)
        - add friends (firend request, accept, delete)
        - add groups (create, join, leave, delete)
        - add chat history 
        - add chat history search
        - add chat history download
        - add chat history delete
*/

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            if (program.checkIfAdmin(args))
            {
                Console.WriteLine("Welcome in admin mode");
                return;
            }
            Console.WriteLine(program.checkDB());
            if (program.checkDB())
            {
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Register");
                    Console.WriteLine("3. Exit");
                    
                    char input = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    
                    switch (input)
                    {
                        case '1':
                            User user = new User(program.login());
                            
                            if (user.getEmail() != null)
                            {
                                user.userMenu();
                            }
                            break;
                        
                        case '2':
                            program.register();
                            break;
                        
                        case '3':
                            return;
                        
                        default:
                            Console.WriteLine("There is no such option");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Couldn't establish connection with database");
                return;
            }
            return;
        }

        bool checkIfAdmin(string[] args)
        {
            foreach (var i in args)
            {
                if (i.Equals("admin"))
                {
                    return true;
                }
            }

            return false;
        }
        
        bool checkDB()
        {
            try
            {
                MySqlConnection conn;
                string connectionString = "server=localhost;uid=user;pwd=12345678;database=Chat;";
                
                conn = new MySqlConnection(connectionString);
                
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return false;
            }
        }

        void register()
        {
            string  ip, email = null, password = null, nick = null;
            MySqlConnection conn;
            string connectionString = "server=localhost;uid=user;pwd=12345678;database=Chat;";
            conn = new MySqlConnection(connectionString);
            ip = getLocalIpAddress();
            
            conn.Open();
            while (true)
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine();

                MySqlCommand comm = conn.CreateCommand();
                
                comm.CommandText = "SELECT Email FROM Users WHERE Email = @mail";
                comm.Parameters.AddWithValue("@mail", email);
                MySqlDataReader reader = comm.ExecuteReader();
                
                if (reader.HasRows)
                {
                    Console.WriteLine("Email already exists");
                    reader.Close();
                    continue;
                }
                
                if (email.Length < 1 || email.Length > 32)
                {
                    Console.WriteLine("Email too short or too long");
                }
                else
                {
                    break;
                }
            }
            conn.Close();
            
            while (true)
            {
                Console.Write("Enter password: ");
                StringBuilder input = new StringBuilder();
                
                while (true)
                {
                    var tmp = Console.ReadKey(true);
                    if (tmp.Key == ConsoleKey.Enter)
                    {
                        password = input.ToString();
                        break;
                    }

                    if (tmp.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        Console.Write("\b \b");
                        input.Remove(input.Length - 1, 1);
                    }
                    else if (tmp.Key != ConsoleKey.Backspace)
                    {
                        Console.Write("*");
                        input.Append(tmp.KeyChar);
                    }
                }

                if (password.Length < 1 || password.Length > 18)
                {
                    Console.WriteLine("password too short or too long");
                }
                else
                {
                    Console.WriteLine();
                    break;
                }
            }
            
            
            while (true)
            {
                Console.Write("Enter nick: ");
                nick = Console.ReadLine();

                if (nick.Length < 1 || nick.Length > 32)
                {
                    Console.WriteLine("Nick too short or too long");
                }
                else
                {
                    break;
                }
            }
            
            try
            {
                conn.Open();

                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "INSERT INTO Users(Email, Password, Nick, IP, Online) VALUES(@mail, @pwd, @nickname, @ipv4, 0)";
                comm.Parameters.AddWithValue("@mail", email);
                comm.Parameters.AddWithValue("@pwd", password);
                comm.Parameters.AddWithValue("@nickname", nick);
                comm.Parameters.AddWithValue("@ipv4", ip);
                comm.ExecuteNonQuery();
                
                conn.Close();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return ;
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        Person? login()
        {
            string email, password;
            string connectionString = "server=localhost;uid=user;pwd=12345678;database=Chat;";
            MySqlConnection conn;
            MySqlCommand comm;

            try
            {
                conn = new MySqlConnection(connectionString);
                int tries = 0;
                
                while (tries < 3)
                {
                    conn.Open();
                    Console.Write("Enter email: ");
                    email = Console.ReadLine();
                    
                    StringBuilder input = new StringBuilder();
                    Console.Write("Enter password: ");
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if(key.Key == ConsoleKey.Enter)
                        {
                            password = input.ToString();
                            break;
                        }
                        if(key.Key == ConsoleKey.Backspace && input.Length > 0)
                        {
                            Console.Write("\b \b");
                            input.Remove(input.Length - 1, 1);
                        }
                        else if(key.Key != ConsoleKey.Backspace)
                        {
                            Console.Write("*");
                            input.Append(key.KeyChar);
                        }
                    }
                    Console.WriteLine();

                    comm = conn.CreateCommand();
                    comm.CommandText = "SELECT Email, Password FROM Users WHERE Email = @mail AND Password = @pwd";
                    comm.Parameters.AddWithValue("@mail", email);
                    comm.Parameters.AddWithValue("@pwd", password);
                    MySqlDataReader reader = comm.ExecuteReader();
                    
                    if (reader.HasRows)
                    {
                        reader.Close();
                        conn.Close();
                        
                        Console.WriteLine("Successfully logged in");
                        conn.Open();
                        
                        comm = conn.CreateCommand();
                        comm.CommandText = "SELECT Email, Password, Nick, IP FROM Users WHERE Email = @mail AND Password = @pwd";
                        comm.Parameters.AddWithValue("@mail", email);
                        comm.Parameters.AddWithValue("@pwd", password);
                        reader = comm.ExecuteReader();
                        reader.Read();
                        Person p1 = new Person(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        
                        reader.Close();
                        conn.Close();
                        
                        return p1;
                    }
                    else
                    {
                        Console.WriteLine("Wrong email or password");
                        tries++;
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            
            Console.WriteLine("Login failed");
            return null;
        }
        
        string getLocalIpAddress()
        {
            string localIp = null;
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIp = endPoint.Address.ToString();
                }

                return localIp;
            }
            catch
            {
                Console.WriteLine("No internet adapter found");
                System.Environment.Exit(-1);
            }

            return localIp;
        }
    }
}