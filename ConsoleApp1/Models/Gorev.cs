namespace ConsoleApp1.Models
{
    public class Gorev
    {
        public int Id { get; set; } // Görev ID'si
        public required string Baslik { get; set; } // Görev başlığı
        public required string Aciklama { get; set; } // Görev açıklaması
        public DateTime OlusturmaTarihi { get; set; } // Oluşturulma tarihi
        public bool TamamlandiMi { get; set; } // Tamamlanma durumu
        public int KullaniciId { get; set; } // Görevin sahibi kullanıcı ID'si

        // Görevi ekrana okunabilir şekilde yazdırır
        public void Yazdir()
        {
            string durum = TamamlandiMi ? "✅ Tamamlandı" : "🕒 Bekliyor";
            Console.WriteLine(
                $"ID: {Id} | Başlık: {Baslik} | Açıklama: {Aciklama} | Tarih: {OlusturmaTarihi:dd.MM.yyyy HH:mm:ss} | Durum: {durum}"
            );
        }
    }
}
