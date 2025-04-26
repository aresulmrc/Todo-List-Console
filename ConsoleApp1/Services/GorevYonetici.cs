using System.Text.Json;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services
{
    public class GorevYonetici
    {
        // Tüm görevler bellekte tutulur
        private readonly List<Gorev> tumGorevler = new();

        // Aktif kullanıcıya ait işlemler yapılır
        private readonly Kullanici aktifKullanici;

        // Görevlerin kaydedildiği dosya yolu
        private const string GorevDosyaYolu = "Tasks.json";

        public Kullanici AktifKullanici => aktifKullanici;

        public GorevYonetici(Kullanici kullanici)
        {
            aktifKullanici = kullanici;
        }

        // Aktif kullanıcıya ait görevler için bir sonraki ID'yi bulur
        private int GetSonrakiGorevId()
        {
            var kullaniciGorevleri = tumGorevler.Where(g => g.KullaniciId == aktifKullanici.Id);
            return kullaniciGorevleri.Any() ? kullaniciGorevleri.Max(g => g.Id) + 1 : 1;
        }

        // Yeni görev ekler
        public bool GorevEkle(string baslik, string aciklama)
        {
            if (string.IsNullOrWhiteSpace(baslik) || string.IsNullOrWhiteSpace(aciklama))
                return false;

            int yeniId = GetSonrakiGorevId();

            Gorev yeniGorev = new Gorev
            {
                Id = yeniId,
                Baslik = baslik,
                Aciklama = aciklama,
                OlusturmaTarihi = DateTime.Now,
                TamamlandiMi = false,
                KullaniciId = aktifKullanici.Id,
            };

            tumGorevler.Add(yeniGorev);
            GorevleriKaydet();
            return true;
        }

        // Görev günceller
        public bool GorevGuncelle(int id, string yeniBaslik, string yeniAciklama)
        {
            Gorev? hedefGorev = tumGorevler.FirstOrDefault(g =>
                g.Id == id && g.KullaniciId == aktifKullanici.Id
            );

            if (hedefGorev != null)
            {
                bool guncellendi = false;
                if (!string.IsNullOrWhiteSpace(yeniBaslik))
                {
                    hedefGorev.Baslik = yeniBaslik;
                    guncellendi = true;
                }
                if (!string.IsNullOrWhiteSpace(yeniAciklama))
                {
                    hedefGorev.Aciklama = yeniAciklama;
                    guncellendi = true;
                }

                if (guncellendi)
                {
                    GorevleriKaydet();
                    return true;
                }
                return false;
            }
            return false;
        }

        // Görevi tamamlandı olarak işaretler
        public bool GorevTamamla(int id)
        {
            Gorev? hedefGorev = tumGorevler.FirstOrDefault(g =>
                g.Id == id && g.KullaniciId == aktifKullanici.Id
            );

            if (hedefGorev != null && !hedefGorev.TamamlandiMi)
            {
                hedefGorev.TamamlandiMi = true;
                GorevleriKaydet();
                return true;
            }
            return false;
        }

        // Aktif kullanıcının görevlerini listeler
        public void GorevleriListele()
        {
            var kullaniciGorevleri = tumGorevler
                .Where(g => g.KullaniciId == aktifKullanici.Id)
                .OrderBy(g => g.Id)
                .ToList();

            if (kullaniciGorevleri.Count == 0)
            {
                Console.WriteLine("\nHenüz hiç görev yok.\n");
                return;
            }

            Console.WriteLine("\nGörev Listesi:");
            foreach (var gorev in kullaniciGorevleri)
            {
                gorev.Yazdir();
            }
            Console.WriteLine();
        }

        // Görev siler ve kalan görevlerin ID'lerini yeniden sıralar
        public bool GorevSil(int id)
        {
            Gorev? hedefGorev = tumGorevler.FirstOrDefault(g =>
                g.Id == id && g.KullaniciId == aktifKullanici.Id
            );

            if (hedefGorev != null)
            {
                tumGorevler.Remove(hedefGorev);
                KullaniciGorevleriniYenidenSirala(aktifKullanici.Id);
                GorevleriKaydet();
                return true;
            }
            return false;
        }

        // Silme sonrası aktif kullanıcının görev ID'lerini 1'den başlayarak sıralar
        private void KullaniciGorevleriniYenidenSirala(int kullaniciId)
        {
            var kullaniciGorevleri = tumGorevler
                .Where(g => g.KullaniciId == kullaniciId)
                .OrderBy(g => g.OlusturmaTarihi)
                .ToList();

            int yeniId = 1;
            foreach (var gorev in kullaniciGorevleri)
            {
                gorev.Id = yeniId++;
            }
        }

        // Görevleri dosyaya kaydeder
        private void GorevleriKaydet()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(
                    tumGorevler,
                    new JsonSerializerOptions { WriteIndented = true }
                );
                File.WriteAllText(GorevDosyaYolu, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚫 Görevler kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        // Görevleri dosyadan yükler ve tüm kullanıcıların görev ID'lerini düzeltir
        public void GorevleriYukle()
        {
            tumGorevler.Clear();
            if (File.Exists(GorevDosyaYolu))
            {
                try
                {
                    string jsonString = File.ReadAllText(GorevDosyaYolu);
                    var yuklenenGorevler = JsonSerializer.Deserialize<List<Gorev>>(jsonString);

                    if (yuklenenGorevler != null)
                    {
                        tumGorevler.AddRange(yuklenenGorevler);
                        KullaniciGorevIdleriniDuzelt();
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine(
                        $"🚫 Görev dosyası okunurken JSON format hatası: {ex.Message}"
                    );
                    tumGorevler.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"🚫 Görevler yüklenirken hata oluştu: {ex.Message}");
                }
            }
        }

        // Tüm kullanıcıların görev ID'lerini 1'den başlayarak sıralar
        private void KullaniciGorevIdleriniDuzelt()
        {
            var kullaniciGruplari = tumGorevler.GroupBy(g => g.KullaniciId);
            bool degisiklikYapildi = false;
            foreach (var grup in kullaniciGruplari)
            {
                int id = 1;
                foreach (var gorev in grup.OrderBy(g => g.OlusturmaTarihi))
                {
                    if (gorev.Id != id)
                    {
                        gorev.Id = id;
                        degisiklikYapildi = true;
                    }
                    id++;
                }
            }
            if (degisiklikYapildi)
            {
                GorevleriKaydet();
            }
        }
    }
}
