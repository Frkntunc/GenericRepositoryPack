#  Generic Application

**Generic Application: Modern & Scalable Boilerplate**
.NET 10 ekosistemi üzerinde inşa edilmiş; esneklik, ölçeklenebilirlik ve yüksek yeniden kullanılabilirlik hedeflenerek tasarlanmış kapsamlı bir API altyapısıdır. Bu proje, bir yazılımın üretim ortamına (production-ready) çıkması için gereken tüm temel ihtiyaçları, SOLID prensiplerinden ödün vermeden tek bir çatı altında toplar.

**Proje Amacı**  
Modern yazılım dünyasında "tekerleği her seferinde yeniden icat etmemek" kritik bir önem taşır. Bu projenin temel amacı; her ölçekteki iş için, güvenliği ve performansı odağına alan, mikroservis dönüşümüne hazır bir başlangıç noktası sağlamaktır.

---

##  Başlıca Özellikler

- Katmanlı mimari
- CQRS ve MediatR ile komut ve sorguların ayrıştırılması  
- JWT Authentication + Refresh Token sistemi  
- Modüller arası gevşek bağ 
- Test altyapısı (Unit)
- Localization altyapısı
- Rate limiting
- Idempotency koruması
- CSRF ve XSS gibi security altyapıları
- Policy bazlı yetkilendirme
- UoW altyapısı
- Log altyapısı
- Cache altyapısı
- Queue altyapısı
- Retry And DeadLetter altyapısı
- **Geliştirmeye Açık Mimari:**  
  - İlerleyen zamanlarda mikroservis mimarisine evrilebilir.  
  - Yeni özellikler kolayca entegre edilebilir.  
  - Alternatif veri kaynakları (SQL, NoSQL) ile hibrit çözümler uygulanabilir 

---

## Mimari ve Teknik Kabiliyetler
Proje, karmaşıklığı yönetmek ve sürdürülebilirliği artırmak için modern mimari desenleri bir araya getirir:

**Çekirdek Mimari**
- Clean Architecture & Layered Approach: Bağımlılıkların merkeze (domain) aktığı, teknoloji bağımsız katmanlı yapı.
- CQRS & MediatR: Komut (Command) ve sorguların (Query) birbirinden ayrıştırılmasıyla sağlanan yüksek okunabilirlik ve performans.
- Unit of Work (UoW) & Repository Pattern: Veri tutarlılığı ve merkezi veri erişim yönetimi.

**Güvenlik ve Dayanıklılık**
- Gelişmiş Kimlik Doğrulama: JWT (JSON Web Token) ve Refresh Token mekanizması ile kesintisiz ve güvenli oturum yönetimi.
- Güvenlik Katmanları: CSRF ve XSS saldırılarına karşı yerleşik koruma; Policy-based yetkilendirme ile granüler erişim kontrolü.
- Resilience (Dayanıklılık): Hata anında otomatik Retry mekanizmaları ve işlenemeyen mesajlar için Dead Letter Queue yönetimi.
- Idempotency & Rate Limiting: Tekrarlanan isteklerin yan etkilerini önleme ve API limitlendirme ile kaynak koruması.

**Altyapı Servisleri**
- Caching & Queue: Performans için merkezi önbellekleme ve asenkron süreç yönetimi için mesaj kuyruğu entegrasyonu.
- Gözlemlenebilirlik (Logging): Uygulama içi tüm olayların izlenebilirliğini sağlayan yapılandırılmış log altyapısı.
- Localization: Çok dilli uygulama desteği için esnek yerelleştirme sistemi.

---

##  Teknoloji Stack

| Katman/Alan            | Teknolojiler / Araçlar             |
|------------------------|------------------------------------|
| Framework              | .NET 10 (ve sonrası)                |
| Mimari Yapı            | Clean Architecture / CQRS           |
| ORM                    | Entity Framework Core              |
| Security               | JWT, Refresh Token, Policy-based Auth    |
| Mimari                 | Clean Architecture |
| Logging                | Serilog |
| Validasyon             | FluentValidation                   |
| Dokümantasyon          | Swagger                     |

