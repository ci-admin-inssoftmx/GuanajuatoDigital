namespace GuanajuatoAdminUsuarios.DBConect
{
    public class DbConect
    {

        public DbConect()

        {
            //DbConection = "Server=10.16.158.32;Database=riag;User =sa;Password=S3guR1t3ch#;Trusted_Connection=False;TrustServerCertificate=True";
            //DbConection2 = "Server=10.16.158.32;Database=riag;User =sa;Password=S3guR1t3ch#;Trusted_Connection=False;TrustServerCertificate=True";
            //DbConection = "Server=10.16.158.17;Database=RiagDevPhase2;User =Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";
            //DbConection2 = "Server=10.16.158.17;Database=RiagDevPhase2;User =Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";

            DbConection = "Server=10.16.158.17;Database=RiagSprint4;User =Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";
            DbConection2 = "Server=10.16.158.17;Database=RiagSprint4;User =Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";
            DbConection3 = "Server=10.16.158.30,1433;Database=GUANAJUATO_LICENCIAS;User Id=useradmin;Password=cV@mlK8lF0$0DlY5u5lE;Trusted_Connection=False;TrustServerCertificate=True";
        }

        //DbConection =  "Server=10.16.158.17;Database=RiagSprint2;User =Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";
        //DbConection2 =  "Server=10.16.158.17;Database=RiagSprint2;User =Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";
        //DbConection3 =  "Server=10.16.158.30,1433;Database=GUANAJUATO_LICENCIAS;User Id=useradmin;Password=cV@mlK8lF0$0DlY5u5lE;Trusted_Connection=False;TrustServerCertificate=True";

        //DbConection = "Server=10.16.158.17;Database=RiagSeg;User=Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";
        //DbConection2 = "Server=10.16.158.17;Database=RiagSeg;User=Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";
        //DbConection3 = "Server=10.16.158.17;Database=RiagSeg;User=Soporte;Password=Seguritech123;Trusted_Connection=False;TrustServerCertificate=True";         
        public readonly string DbConection;
        public readonly string DbConection2;
        public readonly string DbConection3;
    }

}

