
# Chunked File Storage CLI Uygulaması

[![.NET 9](https://img.shields.io/badge/.NET-9-blueviolet)](https://dotnet.microsoft.com)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![Serilog](https://img.shields.io/badge/logging-Serilog-blue)](https://serilog.net/)

Chunk tabanlı dağıtık dosya depolama işlemleri gerçekleştiren, modüler ve genişletilebilir bir **.NET 9 Console Uygulaması**. Büyük dosyalar küçük parçalara ayrılarak birden fazla `StorageProvider` (ör. dosya sistemi, veritabanı vb.) üzerinde saklanır. Sonrasında bu parçalar birleştirilip SHA256 doğrulaması ile bütünlük kontrolü yapılır.

---

## ✨ Özellikler

- 📦 Dosyaların chunk'lara bölünmesi ve birleştirilmesi
- 🔁 Round-robin algoritması ile storage provider seçimi
- 🧩 Esnek yapı: `IStorageProvider` arayüzüne bağlı provider mimarisi
- ✅ SHA256 checksum ile veri bütünlüğü
- 🔧 `appsettings.json` üzerinden yapılandırılabilirlik
- 🔍 Serilog destekli loglama (günlük bazlı dosya logu)
- 🔄 Tekli ve çoklu dosya yükleme/indirme desteği
- 🧪 Repository & Unit of Work tasarımı
- ✅ Clean (Onion) Architecture ile katmanlı yapı

---

## 🏗️ Proje Mimarisi

```
ChunkedFileStorageApp/
│
├── Core/                 # Domain modelleri ve konfigürasyonlar
├── Application/          # Arayüzler ve servisler
├── Infrastructure/       # DbContext, provider'lar, repository’ler
├── CLI/                  # Console uygulama giriş noktası
```

---

## 📁 Storage Provider'lar

Sistem, `IStorageProvider` arayüzü ile birden fazla depolama sağlayıcısını destekler.

- `FileSystemStorageProvider`: Chunk’ları dosya sisteminde saklar
- `DatabaseStorageProvider`: Chunk’ları EF Core in-memory veritabanında saklar

Yükleme işlemleri sırasında provider seçimi round-robin algoritması ile yapılır.

---

## ⚙️ Yapılandırma

CLI projesindeki `appsettings.json` yapılandırması:

```json
{
  "ChunkSettings": {
    "ChunkSizeInBytes": 1048576,
    "StoragePath": "Chunks"
  },
  "Serilog": {
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/log.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

---

## 🧪 CLI Kullanımı

Uygulama çalıştırıldığında menü şöyle görünür:

```
===== Distributed File Storage =====
1. Tekli Dosya Yükle (Chunk + Storage)
2. Çoklu Dosya Yükle (Chunk + Storage)
3. Dosya Birleştir
4. Tüm Dosyaları Listele
5. Chunk Detayları
0. Çıkış
Seçiminiz:
```

Tekli yükleme örneği:
```
Seçiminiz: 1
Dosya yolu:
> C:\Users\ahmet\Documents\example.pdf

Dosya yüklendi. File ID: 0198591b-6679-758c-be46-908627deef57
```

---

## 🧱 Bağımlılık Enjeksiyonu

`InfrastructureServiceExtensions.cs` içinde gerekli servisler DI konteynerine eklenir:

```csharp
services.AddScoped<IFileChunkService, FileChunkService>();
services.AddScoped<IStorageProvider, FileSystemStorageProvider>();
services.AddScoped<IStorageProvider, DatabaseStorageProvider>();
services.AddScoped<IStorageProviderFactory, StorageProviderFactory>();
```

---

## 🛠️ Kullanılan Teknolojiler

- .NET 9
- EF Core (In-Memory Provider)
- UnitOfWork
- Repository Pattern
- DesignTimeDbContextFactory dbFirst
- FactoryDesign Pattern
- Layered Architecture
- Serilog (minLevel=Debug)
- Onion Architecture
- SOLID Prensipleri
- Console App CLI

---


## 👨‍💻 Geliştirici

**Ahmet Aktaş**  
