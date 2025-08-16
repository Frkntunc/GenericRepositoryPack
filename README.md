#  Generic Application (Modular Monolith + Clean Architecture)

**Proje Amacı**  
Bu proje, kişisel projeler ve API çözümleri için esnek, ölçeklenebilir ve yeniden kullanılabilir bir altyapı sağlar. Clean Architecture, CQRS, Modular Monolith ve Vertical Slice yaklaşımlarını bir araya getirerek hem modüler hem de sürdürülebilir bir yapı sunar.

---

##  Neden Bu Yapı?

- **Clean Architecture** kullanımı sayesinde tablo ve kullanıcı arayüzü gibi dış katmanlardan iş mantığını izole eder; böylece test edilebilirlik, bakım ve genişletilebilirlik artar :contentReference[oaicite:3]{index=3}.  
- **Modular Monolith** mimarisi, her bir modülün kendi domain, application, infrastructure ve presentation katmanlarını barındırmasını sağlar; modüller arası bağımlılık minimize edilir, kapsülleme artar :contentReference[oaicite:4]{index=4}.  
- **Vertical Slice Architecture** ile her özellik odaklı tek dosya ya da klasör içinde toplanır; kod karmaşıklığı azalır ve yeni özellik eklemek daha hızlı ve basit olur :contentReference[oaicite:5]{index=5}.

---

##  Başlıca Özellikler

- Katmanlı mimari
- CQRS ve MediatR ile komut ve sorguların ayrıştırılması  
- JWT Authentication + Refresh Token sistemi  
- Modüller arası gevşek bağ 
- Her modülün veri bağımsızlığı (ayrı şema/dosya)  
- Vertical Slice ile özelliğe dayalı kod organizasyonu  
- Test altyapısı (Unit)
- **Geliştirmeye Açık Mimari:**  
  - İlerleyen zamanlarda kolayca mikroservis mimarisine evrilebilir  
  - Yeni modüller eklenerek dikeyde büyüyebilir  
  - Alternatif veri kaynakları (SQL, NoSQL) ile hibrit çözümler uygulanabilir 

---

##  Teknoloji Stack

| Katman/Alan            | Teknolojiler / Araçlar             |
|------------------------|------------------------------------|
| Framework              | .NET 8 (ve sonrası)                |
| CQRS + MediatR         | Komut ve sorgu modelleri           |
| ORM                    | Entity Framework Core              |
| Authentication         | JWT + Refresh Token                |
| Mimari                 | Clean Architecture, Modular Monolith, Vertical Slice |
| Logging / Mapping      | Serilog, Mapster / AutoMapper (isteğe bağlı) |
| Validasyon             | FluentValidation                   |
| Dokümantasyon          | Swagger / NSwag                    |

---

##  Kurulum & Başlangıç

```bash
git clone <repo-url>
cd GenericApp.Infrastructure
dotnet restore
dotnet build
dotnet run --project Presentation/Api
# API çalıştıktan sonra https://localhost:5001/swagger adresinden dokümantasyona ulaşabilirsiniz.
