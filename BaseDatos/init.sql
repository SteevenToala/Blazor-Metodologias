-- Crear la base de datos (opcional)
-- CREATE DATABASE productdb;

-- Conectarse a la base de datos
-- \c productdb

-- Tabla de Suppliers
CREATE TABLE suppliers (
    supplierid SERIAL PRIMARY KEY,
    companyname VARCHAR(40) NOT NULL,
    contactname VARCHAR(50),
    address VARCHAR(100),
    city VARCHAR(50),
    country VARCHAR(50),
    phone VARCHAR(30)
);

-- Tabla de Categories
CREATE TABLE categories (
    categoryid SERIAL PRIMARY KEY,
    categoryname VARCHAR(40) NOT NULL,
    description TEXT
);

-- Tabla de Products
CREATE TABLE products (
    productid SERIAL PRIMARY KEY,
    productname VARCHAR(40) NOT NULL,
    supplierid INTEGER REFERENCES suppliers(supplierid),
    categoryid INTEGER REFERENCES categories(categoryid),
    quantityperunit VARCHAR(20),
    unitprice NUMERIC(10,2) DEFAULT 0 CHECK (unitprice >= 0),
    unitsinstock SMALLINT DEFAULT 0 CHECK (unitsinstock >= 0),
    unitsonorder SMALLINT DEFAULT 0 CHECK (unitsonorder >= 0),
    reorderlevel SMALLINT DEFAULT 0 CHECK (reorderlevel >= 0),
    discontinued BOOLEAN NOT NULL DEFAULT FALSE
);