DROP DATABASE IF EXISTS overdrive;
GO

CREATE DATABASE overdrive;
GO

USE overdrive;
GO

-- STATUS & PAYMENT
CREATE TABLE status (
    id INT IDENTITY PRIMARY KEY,
    description NVARCHAR(50),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME
);

CREATE TABLE payment (
    id INT IDENTITY PRIMARY KEY,
    payment_method NVARCHAR(50),
    description NVARCHAR(500),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME
);

-- ADDRESS
CREATE TABLE address (
    id INT IDENTITY PRIMARY KEY,
    street VARCHAR(120),
    number VARCHAR(20),
    complement VARCHAR(60),
    neighborhood VARCHAR(60),
    city VARCHAR(60),
    state CHAR(2),
    zip CHAR(8),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME
);

-- CUSTOMER
CREATE TABLE customer (
    id INT IDENTITY PRIMARY KEY,
    cpf_cnpj CHAR(14),
    customer_type CHAR(2),
    name NVARCHAR(100),
    phone CHAR(30),
    email VARCHAR(100),
    active BIT,
    fk_address INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,
    FOREIGN KEY (fk_address) REFERENCES address(id)
);

-- VEHICLE
CREATE TABLE vehicle (
    id INT IDENTITY PRIMARY KEY,
    
    chassi VARCHAR(30),
    plate VARCHAR(10),
    brand VARCHAR(50),
    model VARCHAR(80),
    year INT,
    mileage INT,
    condition VARCHAR(20),
    status VARCHAR(20),
    value DECIMAL(18,2),
    fk_address INT NOT NULL,
    fk_customer INT NOT NULL,

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,
    FOREIGN KEY (fk_address) REFERENCES address(id),
    FOREIGN KEY (fk_customer) REFERENCES customer(id)
);

-- BRANCH
CREATE TABLE branch (
    id INT IDENTITY PRIMARY KEY,
    corporate_name NVARCHAR(80),
    cnpj CHAR(14),
    phone CHAR(30),
    email VARCHAR(100),
    active BIT,
    fk_address INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,
    FOREIGN KEY (fk_address) REFERENCES address(id)
);

-- STOCK
CREATE TABLE stock (
    id INT IDENTITY PRIMARY KEY,
    name VARCHAR(100),
    note VARCHAR(500),
    fk_branch INT,
    fk_address INT NOT NULL,

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,
    FOREIGN KEY (fk_address) REFERENCES address(id),
    FOREIGN KEY (fk_branch) REFERENCES branch(id)
);

-- EMPLOYEE
CREATE TABLE employee (
    id INT IDENTITY PRIMARY KEY,
    registration VARCHAR(30),
    name NVARCHAR(100),
    position VARCHAR(30),
    commission_rate DECIMAL(5,4),
    active BIT,
    birth_date DATETIME,
    
    fk_address INT NULL,
    fk_branch INT,
    fk_department INT,

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,
    FOREIGN KEY (fk_address) REFERENCES address(id),
    FOREIGN KEY (fk_branch) REFERENCES branch(id)
);

-- PART
CREATE TABLE part (
    id INT IDENTITY PRIMARY KEY,
    code VARCHAR(50),
    description NVARCHAR(500),
    quantity INT,
    price DECIMAL(18,2),
    fk_stock INT,

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,
    FOREIGN KEY (fk_stock) REFERENCES stock(id)
);

-- SALE
CREATE TABLE sale (
    id INT IDENTITY PRIMARY KEY,
    sale_date DATETIME,
    subtotal DECIMAL(18,2),
    discount DECIMAL(18,2),
    additional_fee DECIMAL(18,2),
    total DECIMAL(18,2),

    fk_customer INT NOT NULL,
    fk_branch INT,
    fk_employee INT,
    fk_payment INT,
    fk_status INT,

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,

    FOREIGN KEY (fk_customer) REFERENCES customer(id),
    FOREIGN KEY (fk_branch) REFERENCES branch(id),
    FOREIGN KEY (fk_employee) REFERENCES employee(id),
    FOREIGN KEY (fk_payment) REFERENCES payment(id),
    FOREIGN KEY (fk_status) REFERENCES status(id)
);

-- SALE_ITEM
CREATE TABLE sale_item (
    id INT IDENTITY PRIMARY KEY,
    item_type VARCHAR(10) NOT NULL, -- 'PART' OR 'VEHICLE'
    quantity INT,
    unit_price DECIMAL(18,2),
    discount DECIMAL(18,2),
    fk_part INT NULL,
    fk_vehicle INT NULL,
    fk_sale INT NOT NULL,
    
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,
    FOREIGN KEY (fk_sale) REFERENCES sale(id),
    FOREIGN KEY (fk_part) REFERENCES part(id),
    FOREIGN KEY (fk_vehicle) REFERENCES vehicle(id)
);

-- SERVICE_ORDER
CREATE TABLE service_order (
    id INT IDENTITY PRIMARY KEY,
    number VARCHAR(30),
    open_date DATETIME,
    close_date DATETIME,
    status VARCHAR(20),
    total_value DECIMAL(18,2),
    notes VARCHAR(500),

    fk_vehicle INT,
    fk_branch INT,
    
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,

    FOREIGN KEY (fk_vehicle) REFERENCES vehicle(id),
    FOREIGN KEY (fk_branch) REFERENCES branch(id)
);

-- SERVICE_ORDER_ITEM
CREATE TABLE service_order_item (
    id INT IDENTITY PRIMARY KEY,
    description VARCHAR(300),
    quantity INT,
    price DECIMAL(18,2),
    subtotal DECIMAL(18,2),
    fk_service_order INT,

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    edited_at DATETIME,

    FOREIGN KEY (fk_service_order) REFERENCES service_order(id)
);

-- USE (part ? service_order_item)
CREATE TABLE use_part (
    fk_part INT,
    fk_service_order_item INT,
    quantity INT,
    PRIMARY KEY (fk_part, fk_service_order_item),
    FOREIGN KEY (fk_part) REFERENCES part(id),
    FOREIGN KEY (fk_service_order_item) REFERENCES service_order_item(id)
);