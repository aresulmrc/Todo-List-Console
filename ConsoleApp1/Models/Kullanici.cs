namespace ConsoleApp1.Models
{
    public class Kullanici
    {
        public int Id { get; set; } // Kullanıcı ID'si
        public required string KullaniciAdi { get; set; } // Kullanıcı adı
        public required string Sifre { get; set; } // Hash'lenmiş şifre
    }
}
