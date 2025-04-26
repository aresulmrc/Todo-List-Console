using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services
{
    public class KullaniciYonetici
    {
        // Tüm kullanıcılar bellekte tutulur
        private readonly List<Kullanici> kullanicilar = new();
        private int siradakiId = 1;

        // Kullanıcıların kaydedildiği dosya yolu
        private const string KullaniciDosyaYolu = "Users.json";

        public Kullanici? AktifKullanici { get; private set; }

        public KullaniciYonetici()
        {
            KullanicilariYukle();
        }

        // Şifreyi SHA256 ile hash'ler
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Yeni kullanıcı kaydı ekler
        public bool KayitOl(string kullaniciAdi, string sifre)
        {
            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
                return false;

            if (
                kullanicilar.Exists(k =>
                    k.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase)
                )
            )
                return false;

            string sifreHash = HashPassword(sifre);

            Kullanici yeniKullanici = new Kullanici
            {
                Id = siradakiId++,
                KullaniciAdi = kullaniciAdi,
                Sifre = sifreHash,
            };

            kullanicilar.Add(yeniKullanici);
            KullaniciKaydet();
            return true;
        }

        // Kullanıcı giriş işlemi
        public Kullanici? GirisYap(string kullaniciAdi, string sifre)
        {
            string girilenSifreHash = HashPassword(sifre);

            Kullanici? kullanici = kullanicilar.Find(k =>
                k.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase)
                && k.Sifre == girilenSifreHash
            );

            if (kullanici != null)
            {
                AktifKullanici = kullanici;
                return kullanici;
            }
            return null;
        }

        // Kullanıcı çıkış işlemi
        public void CikisYap()
        {
            AktifKullanici = null;
        }

        // Kayıtlı kullanıcıları listeler
        public void KullanicilariListele()
        {
            Console.WriteLine("=== Kullanıcı Listesi ===");
            if (!kullanicilar.Any())
            {
                Console.WriteLine("Kayıtlı kullanıcı bulunmuyor.");
                return;
            }
            foreach (var kullanici in kullanicilar)
            {
                Console.WriteLine($"ID: {kullanici.Id}, Kullanıcı Adı: {kullanici.KullaniciAdi}");
            }
            Console.WriteLine();
        }

        // Kullanıcıları dosyaya kaydeder
        private void KullaniciKaydet()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(
                    kullanicilar,
                    new JsonSerializerOptions { WriteIndented = true }
                );
                File.WriteAllText(KullaniciDosyaYolu, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚫 Kullanıcılar kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        // Kullanıcıları dosyadan yükler
        private void KullanicilariYukle()
        {
            kullanicilar.Clear();
            if (File.Exists(KullaniciDosyaYolu))
            {
                try
                {
                    string jsonString = File.ReadAllText(KullaniciDosyaYolu);
                    var yuklenenKullanicilar = JsonSerializer.Deserialize<List<Kullanici>>(
                        jsonString
                    );

                    if (yuklenenKullanicilar != null)
                    {
                        kullanicilar.AddRange(yuklenenKullanicilar);
                        if (kullanicilar.Any())
                        {
                            siradakiId = kullanicilar.Max(k => k.Id) + 1;
                        }
                        else
                        {
                            siradakiId = 1;
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine(
                        $"🚫 Kullanıcı dosyası okunurken JSON format hatası: {ex.Message}"
                    );
                    kullanicilar.Clear();
                    siradakiId = 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"🚫 Kullanıcılar yüklenirken hata oluştu: {ex.Message}");
                }
            }
        }
    }
}
