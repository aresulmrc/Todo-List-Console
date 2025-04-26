using ConsoleApp1.Services;

namespace ConsoleApp1
{
    public class Program
    {
        // Programın giriş noktası
        static void Main(string[] args)
        {
            var kullaniciYonetici = new KullaniciYonetici();

            // Ana program döngüsü: Kullanıcı girişi ve görev yönetimi arasında geçiş yapar
            while (true)
            {
                // Kullanıcı giriş yapana kadar kullanıcı menüsünü göster
                while (kullaniciYonetici.AktifKullanici == null)
                {
                    KullaniciMenu(kullaniciYonetici);
                    // Çıkış seçildiyse programı sonlandır
                    if (kullaniciYonetici.AktifKullanici == null && Environment.ExitCode != 0)
                        return;
                }

                // Giriş başarılıysa görev yöneticisini oluştur ve görevleri yükle
                var gorevYonetici = new GorevYonetici(kullaniciYonetici.AktifKullanici!);
                gorevYonetici.GorevleriYukle();

                // Kullanıcı çıkış yapana veya kullanıcı değiştirene kadar ana menüyü göster
                while (kullaniciYonetici.AktifKullanici != null)
                {
                    if (!AnaMenu(gorevYonetici, kullaniciYonetici))
                        break;
                }
            }
        }

        // Kullanıcı giriş/kayıt/çıkış menüsü
        static void KullaniciMenu(KullaniciYonetici kullaniciYonetici)
        {
            Console.Clear();
            Console.WriteLine("=== Kullanıcı İşlemleri ===");
            Console.WriteLine("1 - Giriş Yap");
            Console.WriteLine("2 - Kayıt Ol");
            Console.WriteLine("3 - Kullanıcıları Listele");
            Console.WriteLine("4 - Çıkış");
            Console.Write("Seçiminiz (1-4): ");
            var secim = Console.ReadLine();

            switch (secim)
            {
                case "1":
                    Console.Write("Kullanıcı Adı: ");
                    var kullaniciAdi = Console.ReadLine();
                    Console.Write("Şifre: ");
                    var sifre = Console.ReadLine();
                    var girisYapanKullanici = kullaniciYonetici.GirisYap(
                        kullaniciAdi ?? "",
                        sifre ?? ""
                    );
                    if (girisYapanKullanici != null)
                    {
                        Console.WriteLine(
                            $"\n👋 Hoş geldin, {girisYapanKullanici.KullaniciAdi}!\n"
                        );
                        Console.WriteLine("Görev menüsüne yönlendiriliyorsunuz...");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("\n🚫 Kullanıcı adı veya şifre yanlış.\n");
                        Console.WriteLine("Devam etmek için bir tuşa basın...");
                        Console.ReadKey();
                    }
                    break;
                case "2":
                    Console.Write("Yeni Kullanıcı Adı: ");
                    var yeniKullaniciAdi = Console.ReadLine();
                    Console.Write("Yeni Şifre: ");
                    var yeniSifre = Console.ReadLine();
                    bool kayitBasarili = kullaniciYonetici.KayitOl(
                        yeniKullaniciAdi ?? "",
                        yeniSifre ?? ""
                    );
                    if (kayitBasarili)
                    {
                        Console.WriteLine("\n✅ Kayıt başarılı! Şimdi giriş yapabilirsiniz.\n");
                    }
                    else
                    {
                        Console.WriteLine(
                            "\n🚫 Kayıt başarısız. Kullanıcı adı zaten alınmış veya geçersiz girdi.\n"
                        );
                    }
                    Console.WriteLine("Devam etmek için bir tuşa basın...");
                    Console.ReadKey();
                    break;
                case "3":
                    kullaniciYonetici.KullanicilariListele();
                    Console.WriteLine("Devam etmek için bir tuşa basın...");
                    Console.ReadKey();
                    break;
                case "4":
                    Console.WriteLine("Programdan çıkılıyor...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("⚠ Geçersiz seçim.");
                    Console.WriteLine("Devam etmek için bir tuşa basın...");
                    Console.ReadKey();
                    break;
            }
        }

        // Ana görev menüsü
        static bool AnaMenu(GorevYonetici gorevYonetici, KullaniciYonetici kullaniciYonetici)
        {
            Console.Clear();
            Console.WriteLine($"=== Hoş Geldin, {gorevYonetici.AktifKullanici.KullaniciAdi} ===");
            Console.WriteLine("=== To-Do List Uygulaması ===");
            Console.WriteLine("1 - Görev Ekle");
            Console.WriteLine("2 - Görevleri Listele");
            Console.WriteLine("3 - Görev Güncelle");
            Console.WriteLine("4 - Görev Sil");
            Console.WriteLine("5 - Görevi Tamamla");
            Console.WriteLine("6 - Kullanıcı Değiştir");
            Console.WriteLine("7 - Çıkış");
            Console.Write("Seçiminizi yapınız (1-7): ");
            var secim = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (secim)
                {
                    case "1":
                        Console.Write("Görev Başlığı: ");
                        var baslik = Console.ReadLine();
                        Console.Write("Açıklama: ");
                        var aciklama = Console.ReadLine();
                        if (gorevYonetici.GorevEkle(baslik ?? "", aciklama ?? ""))
                            Console.WriteLine("\n✅ Görev başarıyla eklendi!\n");
                        else
                            Console.WriteLine(
                                "\n⚠ Görev eklenemedi. Başlık ve açıklama boş olamaz.\n"
                            );
                        break;
                    case "2":
                        gorevYonetici.GorevleriListele();
                        break;
                    case "3":
                        gorevYonetici.GorevleriListele();
                        Console.Write("Güncellenecek Görev ID: ");
                        if (int.TryParse(Console.ReadLine(), out int guncelleId))
                        {
                            Console.Write("Yeni Başlık (Boş bırakmak için Enter): ");
                            var yeniBaslik = Console.ReadLine();
                            Console.Write("Yeni Açıklama (Boş bırakmak için Enter): ");
                            var yeniAciklama = Console.ReadLine();
                            if (
                                gorevYonetici.GorevGuncelle(
                                    guncelleId,
                                    yeniBaslik ?? "",
                                    yeniAciklama ?? ""
                                )
                            )
                            {
                                Console.WriteLine("\n🔄 Görev güncellendi.\n");
                                gorevYonetici.GorevleriListele();
                            }
                            else
                            {
                                Console.WriteLine(
                                    "\n⚠ Görev bulunamadı, size ait değil veya güncellenecek bilgi girilmedi.\n"
                                );
                            }
                        }
                        else
                        {
                            Console.WriteLine("⚠ Lütfen geçerli bir sayı girin.");
                        }
                        break;
                    case "4":
                        gorevYonetici.GorevleriListele();
                        Console.Write("Silinecek Görev ID: ");
                        if (int.TryParse(Console.ReadLine(), out int silId))
                        {
                            if (gorevYonetici.GorevSil(silId))
                            {
                                Console.WriteLine(
                                    "\n🗑 Görev silindi ve ID'ler yeniden düzenlendi.\n"
                                );
                                gorevYonetici.GorevleriListele();
                            }
                            else
                            {
                                Console.WriteLine(
                                    "\n⚠ Silinecek görev bulunamadı veya size ait değil.\n"
                                );
                            }
                        }
                        else
                        {
                            Console.WriteLine("⚠ Lütfen geçerli bir sayı girin.");
                        }
                        break;
                    case "5":
                        gorevYonetici.GorevleriListele();
                        Console.Write("Tamamlanacak Görev ID: ");
                        if (int.TryParse(Console.ReadLine(), out int tamamId))
                        {
                            if (gorevYonetici.GorevTamamla(tamamId))
                            {
                                Console.WriteLine("\n👍 Görev tamamlandı olarak işaretlendi.\n");
                                gorevYonetici.GorevleriListele();
                            }
                            else
                            {
                                Console.WriteLine(
                                    "\n⚠ Görev bulunamadı, size ait değil veya zaten tamamlanmış.\n"
                                );
                            }
                        }
                        else
                        {
                            Console.WriteLine("⚠ Lütfen geçerli bir sayı girin.");
                        }
                        break;
                    case "6":
                        Console.WriteLine("Kullanıcı değiştiriliyor...");
                        kullaniciYonetici.CikisYap();
                        return false;
                    case "7":
                        Console.WriteLine("Programdan çıkılıyor...");
                        Environment.Exit(0);
                        return false;
                    default:
                        Console.WriteLine(
                            "⚠ Geçersiz seçim yaptınız. Lütfen 1-7 arasında bir sayı girin."
                        );
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n🚫 Beklenmeyen bir hata oluştu: {ex.Message}\n");
            }

            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
            return true;
        }
    }
}
