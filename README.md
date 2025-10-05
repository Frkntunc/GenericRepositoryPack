#  Generic Application (Modular Monolith + Clean Architecture)

**Proje Amacı**  
Bu proje, kişisel projeler ve API çözümleri için esnek, ölçeklenebilir ve yeniden kullanılabilir bir altyapı sağlar. Clean Architecture, CQRS ve Modular Monolith yaklaşımlarını bir araya getirerek hem modüler hem de sürdürülebilir bir yapı sunar.

---

##  Neden Bu Yapı?

- **Clean Architecture** kullanımı sayesinde tablo ve kullanıcı arayüzü gibi dış katmanlardan iş mantığını izole eder; böylece test edilebilirlik, bakım ve genişletilebilirlik artar.  
- **Modular Monolith** mimarisi, her bir modülün kendi domain, application, infrastructure ve presentation katmanlarını barındırmasını sağlar; modüller arası bağımlılık minimize edilir, kapsülleme artar.

---

##  Başlıca Özellikler

- Katmanlı mimari
- CQRS ve MediatR ile komut ve sorguların ayrıştırılması  
- JWT Authentication + Refresh Token sistemi  
- Modüller arası gevşek bağ 
- Her modülün veri bağımsızlığı (ayrı şema/dosya)  
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
| Mimari                 | Clean Architecture, Modular Monolith |
| Logging / Mapping      | Serilog / AutoMapper (isteğe bağlı) |
| Validasyon             | FluentValidation                   |
| Dokümantasyon          | Swagger                     |

