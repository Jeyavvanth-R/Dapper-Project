namespace TestSPInsertFinal.Models
{
        public class User
        {
            public Guid userID { get; set; }
            public int serialNumber{ get; set; }
            public string firstName { get; set; } = String.Empty;
            public string lastName { get; set; } = String.Empty;
            public string email { get; set; } = String.Empty;
            public string password { get; set; } = String.Empty;
            public  UserDetails UserDetails{ get; set; }= new UserDetails();
        }

        public class UserDetails
        {          
            public string email { get; set; }= String.Empty;
            public int serialNumber { get; set; }
            public string address { get; set; } = String.Empty;
            public string city { get; set; } = String.Empty;
            public string state { get; set; } = String.Empty;
            public string postalCode { get; set; } = String.Empty;
            public DateTime DOB { get; set; }
            public int age { get; set; }
            public string contactNumber { get; set; } = String.Empty;

        }
    public class Email
    {
        public string email { get; set; } = String.Empty;
    }


}
