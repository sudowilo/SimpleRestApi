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
    invoice_number INT NOT NULL UNIQUE,
    customer_name NVARCHAR(255),
    customer_phone NVARCHAR(50),
    invoice_date DATE NOT NULL,
    status_id INT NOT NULL,
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
