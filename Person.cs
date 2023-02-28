namespace Chat;

class Person
{
    protected string email;
    protected string password;
    protected string nick;
    protected string ip;
    
    public Person()
    {
        email = null;
        password = null;
        nick = null;
        ip = null;
    }
    
    public Person(string email, string password, string nick, string ip)
    {
        this.email = email;
        this.password = password;
        this.nick = nick;
        this.ip = ip;
    }

    ~Person()
    {
        email = null;
        password = null;
        nick = null;
        ip = null;
    }
    
    public string getEmail()
    {
        return email;
    }
    
    public string getPassword()
    {
        return password;
    }
    
    public string getNick()
    {
        return nick;
    }
    
    public string getIp()
    {
        return ip;
    }
    
    public void setEmail(string email)
    {
        this.email = email;
    }
    
    public void setPassword(string password)
    {
        this.password = password;
    }
    
    public void setNick(string nick)
    {
        this.nick = nick;
    }
    
    public void setIp(string ip)
    {
        this.ip = ip;
    }
}