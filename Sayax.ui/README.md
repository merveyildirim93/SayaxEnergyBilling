# Sayax Energy Billing System

Sayax, enerji faturalandırma sürecini dijitalleştiren, şeffaf ve ölçülebilir hale getiren bir uygulamadır. Bu proje, müşteri tüketim verileri üzerinden fatura hesaplamaları ve belediyelere ödenecek BTV tutarlarını otomatik olarak hesaplayan bir yapı sunar.

## 📄 Proje İçeriği

* Müşteri fatura hesaplaması
* Belediye bazlı BTV hesaplama
* CORS destekli .NET Core 8 REST API
* Angular 17 UI katmanı (component + service yapısı)
* xUnit test altyapısı

---

## 📊 Hesaplama Formülleri

### 📅 Tarife Tipleri:

1. **Sanayi (Tarife: S1)**

   * Satış Yönetimi: (PTF + YEK) %Komisyon
2. **Ticarethane (Tarife: S2)**

   * Satış Yönetimi: PTF + YEK + Sabit Komisyon (TL)
3. **Sanayi - İndirimli (Tarife: S3)**

   * Satış Yönetimi: Tarife - %İndirim

### 🔢 Kullanılan Formüller

#### Enerji Bedeli:

```csharp
energyCost = totalConsumption * unitPrice;
```

Tarife türüne göre `unitPrice` hesaplanır.

#### Dağıtım Bedeli:

```csharp
distributionCost = totalConsumption * distributionUnitPrice;
```

#### BTV:

```csharp
btv = energyCost * btvRate;
```

#### KDV:

```csharp
kdv = (energyCost + distributionCost + btv) * kdvRate;
```

#### Toplam:

```csharp
totalInvoice = energyCost + distributionCost + btv + kdv;
```

---

## 📁 Proje Yapısı (Katmanlar)

* **Sayax.Api**

  * REST API Controller'ı (InvoiceController, TaxController)
  * Swagger entegrasyonu
* **Sayax.Application**

  * Business logic (InvoiceService, TaxService)
* **Sayax.Domain**

  * Entity modelleri: `Customer`, `Meter`, `StaticPrices`
* **Sayax.Infrastructure**

  * EF Core ile MSSQL veri erişimi
* **Sayax.Test**

  * xUnit ile Invoice ve Tax hesaplama testleri
* **Sayax.UI**

  * Angular frontend (InvoiceComponent, MunicipalityComponent, HomeComponent)

---

## 🔧 UI Katmanı

* **Ana Sayfa:**

  * "Müşteri Faturalarını Hesapla" butonu
  * "Belediye Tutarlarını Hesapla" butonu

* **Fatura Hesaplama Ekranı**

  * Müşteri seçimi (selectbox)
  * Tarih girişi (input type="date")
  * Hesapla butonu → API çağrısı
  * Sonuçlar: Enerji, Dağıtım, BTV, KDV, Toplam

* **Belediye Tutarları Ekranı**

  * Tarih girişi
  * API çağrısı ile ilgili belediyeler bazında toplam BTV tutarı listesi

---

## 📢 API Endpointleri

### ✉️ `POST /api/invoice/calculate`

> Girdi: `{ customerId, month }`
> Dönüş: `InvoiceResponse`

### ✉️ `GET /api/tax/GetBtvReport?month=yyyy-MM-dd`

> Dönüş: `MunicipalityTax[]`

---

## ✅ Örnek Girdi / Çıktı

### Girdi:

```json
{
  "customerId": 1,
  "month": "2024-07-01"
}
```

### Çıktı:

```json
{
  "customerId": 1,
  "customerName": "Test Müşterisi",
  "energyCost": 6395078.29,
  "distributionCost": 2371200.1,
  "btv": 63951.58,
  "kdv": 883023,
  "totalInvoice": 9713252.97
}
```

---

## 📚 Kurulum

### Backend (.NET 8)

```bash
dotnet build
cd Sayax.Api
dotnet run
```

### Frontend (Angular 17)

```bash
npm install
ng serve
```

> Backend: [https://localhost:7289](https://localhost:7289)
> Frontend: [http://localhost:4200](http://localhost:4200)

---

## 📅 Şirket ve Sektör Bağlamı

Bu proje, enerji sektöründe faaliyet gösteren **Sayax** adlı şirkete teknik case çalışması olarak geliştirilmiştir. Proje EPİAŞ, PTF, YEK gibi sektörel kavramları referans alarak şeffaf faturalandırma ve raporlama hedefiyle tasarlanmıştır.

---

## ✉️ İletişim

Herhangi bir sorunuz olursa benimle iletişime geçmekten çekinmeyin.
Teşekkürler ❤️
