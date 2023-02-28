using System.Text;
using MySqlConnector;

namespace Chat;

class User : Person
{
    private string connectionString = "server=localhost;uid=user;database=Chat;pwd=12345678";
    private Person[] friends;
    
    string getConnectionString()
    {
        return connectionString;
    }
    public void userMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome " + nick);

        while (true)
        {
            char input;
            Console.WriteLine();
            Console.WriteLine("1. Open chat");
            Console.WriteLine("2. Menage your account");
            Console.WriteLine("3. Logout");
            
            input = Console.ReadKey().KeyChar;

            switch (input)
            {
                case '1':
                    break;
                
                case '2':
                    if (menageAccount())
                        return;
                    break;
                
                case '3':
                    return;
                
                default:
                    Console.WriteLine("There is no such option");
                    break;
            }
        }
    }
    
    public User()
    {
        email = null;
        password = null;
        nick = null;
        ip = null;
    }
    public User(Person person)
    {
        if(person != null)
        {
            email = person.getEmail();
            password = person.getPassword();
            nick = person.getNick();
            ip = person.getIp();
        }
        else
        {
            email = null;
            password = null;
            nick = null;
            ip = null;   
        }
    }

    public bool menageAccount()
    {
        var user = new User();

        while (true)
        {
            char input;
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("1. Change password");
            Console.WriteLine("2. Change nick");
            Console.WriteLine("3. Change email");
            Console.WriteLine("4. Delete account");
            Console.WriteLine("5. Back");

            input = Console.ReadKey().KeyChar;

            switch (input)
            {
                case '1':
                    Console.WriteLine();
                    changePassword();
                    break;
                
                case '2':
                    Console.WriteLine();
                    changeNick();
                    break;
                
                case '3':
                    Console.WriteLine();
                    changeEmail();
                    break;
                
                case '4':
                    Console.WriteLine();
                    
                    if (deleteAccount())
                        return true;
                    
                    break;
                
                case '5':
                    return false;
                
                default:
                    Console.WriteLine("There is no such option");
                    break;
            }
        }
    }

    public bool deleteAccount()
    {
        char input;
        
        Console.WriteLine("Are you sure you want to delete your account? (y/n)");
        input = Console.ReadKey().KeyChar;

        if (input == 'y')
        {
            if (checkPwd())
            {
                MySqlConnection conn = new MySqlConnection(getConnectionString());

                try
                {
                    conn.Open();
                    
                    string sql = "DELETE FROM Users WHERE Email = @email";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.ExecuteNonQuery();
                    
                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadKey();
                    return false;
                }
                
                Console.WriteLine("Account deleted");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public void changeEmail()
    {
        if (!checkPwd())
        {
            Console.WriteLine("You have entered wrong password 3 times");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            return;
        }
        else
        {
            string newEmail;

            while (true)
            {
                Console.Write("Enter new email: ");
                newEmail = Console.ReadLine();

                if (checkEmail(newEmail))
                {
                    break;
                }
            }
            
            MySqlConnection conn = new MySqlConnection(getConnectionString());

            try
            {
                conn.Open();
                
                string sql = "UPDATE Users SET Email = @newEmail WHERE Email = @email";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@newEmail", newEmail);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
                
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            email = newEmail;
        }
        
        Console.WriteLine("Email changed");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    public void changeNick()
    {
        string newNick = null;

        if (!checkPwd())
        {
            Console.WriteLine("You have entered wrong password 3 times");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            return;
        }
        else
        {
            Console.Write("Enter new nick: ");
            newNick = Console.ReadLine();
            nick = newNick;
            
            MySqlConnection conn = new MySqlConnection(getConnectionString());

            try
            {
                conn.Open();
                
                string sql = "UPDATE Users SET Nick = @nick WHERE Email = @email";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nick", newNick);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
                
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return;
            }
            
            Console.WriteLine("Nick changed");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
    public void changePassword()
    {
        if(!checkPwd())
        {
            Console.WriteLine("You have entered wrong password 3 times");
            Console.WriteLine("press any key to continue...");
            Console.ReadKey();
            
            return;
        }
        else
        {
            Console.Write("Enter new password: ");
            
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var tmp = Console.ReadKey(true);

                if (tmp.Key == ConsoleKey.Enter)
                {
                    password = input.ToString();
                    Console.WriteLine();
                    break;
                }

                if (tmp.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    Console.Write("\b \b");
                    input.Remove(input.Length - 1, 1);
                }
                else if(tmp.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    input.Append(tmp.KeyChar);
                }
            }
            
            MySqlConnection conn = new MySqlConnection(getConnectionString());
            try
            {
                conn.Open();
                
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "UPDATE Users SET Password = @password WHERE Email = @email";
                comm.Parameters.AddWithValue("@password", password);
                comm.Parameters.AddWithValue("@email", email);
                comm.ExecuteNonQuery();
                
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
            
            Console.WriteLine("Password changed");
            Console.WriteLine("Enter any key to continue");
            Console.ReadKey();
        }
    }

    public bool checkPwd()
    {
        int tries = 3;
        string pwd = null;
        StringBuilder input = new StringBuilder();
        
        while (tries != 0 && pwd != password)
        {
            Console.WriteLine();
            input.Clear();
            Console.Write("Enter password: ");
            
            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    pwd = input.ToString();
                    Console.WriteLine();
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
            tries--;
        }

        if (pwd == password)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool checkEmail(string newEmail)
    {
        MySqlConnection conn = new MySqlConnection(getConnectionString());

        try
        {
            conn.Open();

            string sql = "SELECT Email FROM Users WHERE Email = @email";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            
            cmd.Parameters.AddWithValue("@email", newEmail);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("Email already exists");
                Console.WriteLine();
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}