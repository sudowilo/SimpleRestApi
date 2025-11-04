DROP TABLE IF EXISTS invoice_item;
DROP TABLE IF EXISTS invoices;
DROP TABLE IF EXISTS items;
DROP TABLE IF EXISTS invoice_status;
GO

CREATE TABLE invoice_status (
    id INT IDENTITY(1,1) PRIMARY KEY,
    code NVARCHAR(50) NOT NULL UNIQUE,
    description NVARCHAR(255),
    created_at DATETIME2 DEFAULT SYSDATETIME()
);
GO

CREATE TABLE items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    price BIGINT NOT NULL,
    created_at DATETIME2 DEFAULT SYSDATETIME()
);
GO

CREATE TABLE invoices (
    id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_number NVARCHAR(50) UNIQUE,
    customer_name NVARCHAR(255) NOT NULL,
    customer_phone NVARCHAR(50) NOT NULL,
    invoice_date DATE,
    status_id INT,
    total_amount BIGINT,
    note NVARCHAR(MAX),
    CONSTRAINT fk_invoices_status FOREIGN KEY (status_id)
        REFERENCES invoice_status(id)
);
GO

CREATE TABLE invoice_item (
    id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id INT NOT NULL,
    item_id INT NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    fee BIGINT NOT NULL,
    total_price AS (quantity * fee) PERSISTED,
    CONSTRAINT fk_invoiceitem_invoice FOREIGN KEY (invoice_id)
        REFERENCES invoices(id),
    CONSTRAINT fk_invoiceitem_item FOREIGN KEY (item_id)
        REFERENCES items(id)
);
GO

INSERT INTO items (name, description, price)
VALUES
(N'لپ‌تاپ لنوو ThinkPad E14', N'لپ‌تاپ مخصوص کارهای اداری و تجاری با پردازنده Core i5 و ۸ گیگابایت رم.', 42000000),
(N'مانیتور سامسونگ ۲۴ اینچ', N'نمایشگر LED با کیفیت Full HD مناسب محیط‌های کاری.', 8500000),
(N'ماوس بی‌سیم لاجیتک', N'ماوس بی‌سیم با دقت بالا و طراحی ارگونومیک.', 750000),
(N'کیبورد مکانیکی تسکو', N'کیبورد مکانیکی با نور پس‌زمینه و کلیدهای نرم.', 950000),
(N'پرینتر لیزری اچ‌پی', N'پرینتر تک‌رنگ لیزری با سرعت چاپ بالا مناسب دفاتر.', 5200000),
(N'صندلی اداری ارگونومیک', N'صندلی چرخ‌دار با پشتی توری و قابلیت تنظیم ارتفاع.', 3200000),
(N'میز کار MDF ساده', N'میز اداری ۱۴۰ سانتی‌متری از جنس MDF با کشو.', 2500000),
(N'کابل شبکه CAT6 پنج متری', N'کابل شبکه با سرعت انتقال بالا مناسب شبکه‌های سازمانی.', 180000),
(N'هارد اکسترنال وسترن ۱ ترابایت', N'حافظه اکسترنال USB3 مناسب بکاپ‌گیری اطلاعات.', 3100000),
(N'سرویس پشتیبانی نرم‌افزاری سالانه', N'پشتیبانی و نگهداری سیستم‌های نرم‌افزاری به مدت یک سال.', 6000000);

-- CREATE TRIGGER trg_update_invoice_total
-- ON invoice_item
-- AFTER INSERT, UPDATE, DELETE
-- AS
-- BEGIN
--     UPDATE invoices
--     SET total_amount = ISNULL((
--     SELECT SUM(total_price)
--     FROM invoice_item
--     WHERE invoice_id = invoices.id
-- ), 0)
--     WHERE id IN (SELECT DISTINCT invoice_id FROM inserted UNION SELECT DISTINCT invoice_id FROM deleted);
-- END;

INSERT INTO invoice_status (code, description) 
VALUES 
  ('unpaid', 'default when invoice creates'),
  ('paid', 'after customer paid the price'),
  ('cancelled', 'invoice is not valid any more because it get cancelled for any reason');
