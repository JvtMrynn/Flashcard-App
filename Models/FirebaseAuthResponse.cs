using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardApp.Models
{
    public class FirebaseAuthResponse
    {
        public string idToken { get; set; }
        public string email { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string localId { get; set; }
        public FirebaseError error { get; set; }
    }

    public class FirebaseError
    {
        public FirebaseErrorDetail error { get; set; }
    }

    public class FirebaseErrorDetail
    {
        public string message { get; set; }
    }
}
