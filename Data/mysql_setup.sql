/*
--------------------------------------------------------------------
  2017 sqlservertutorial.net All Rights Reserved
--------------------------------------------------------------------
Name   : BikeStores
Link   : http://www.sqlservertutorial.net/load-sample-database/
Version: 1.0
--------------------------------------------------------------------
*/ 

-- Creo prima il database sales perché production.stocks ha FK verso sales.stores
CREATE DATABASE IF NOT EXISTS sales;
CREATE DATABASE IF NOT EXISTS production;

-- ============================================
-- SALES TABLES (prima, perché stocks le referenzia)
-- ============================================

USE sales;

CREATE TABLE IF NOT EXISTS customers (
	customer_id INT AUTO_INCREMENT PRIMARY KEY,
	first_name VARCHAR(255) NOT NULL,
	last_name VARCHAR(255) NOT NULL,
	phone VARCHAR(25),
	email VARCHAR(255) NOT NULL,
	street VARCHAR(255),
	city VARCHAR(50),
	state VARCHAR(25),
	zip_code VARCHAR(5)
);

CREATE TABLE IF NOT EXISTS stores (
	store_id INT AUTO_INCREMENT PRIMARY KEY,
	store_name VARCHAR(255) NOT NULL,
	phone VARCHAR(25),
	email VARCHAR(255),
	street VARCHAR(255),
	city VARCHAR(255),
	state VARCHAR(10),
	zip_code VARCHAR(5)
);

CREATE TABLE IF NOT EXISTS staffs (
	staff_id INT AUTO_INCREMENT PRIMARY KEY,
	first_name VARCHAR(50) NOT NULL,
	last_name VARCHAR(50) NOT NULL,
	email VARCHAR(255) NOT NULL UNIQUE,
	phone VARCHAR(25),
	active TINYINT NOT NULL,
	store_id INT NOT NULL,
	manager_id INT,
	FOREIGN KEY (store_id) REFERENCES stores (store_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (manager_id) REFERENCES staffs (staff_id) ON DELETE NO ACTION ON UPDATE NO ACTION
);

-- ============================================
-- PRODUCTION TABLES
-- ============================================

USE production;

CREATE TABLE IF NOT EXISTS categories (
	category_id INT AUTO_INCREMENT PRIMARY KEY,
	category_name VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS brands (
	brand_id INT AUTO_INCREMENT PRIMARY KEY,
	brand_name VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS products (
	product_id INT AUTO_INCREMENT PRIMARY KEY,
	product_name VARCHAR(255) NOT NULL,
	brand_id INT NOT NULL,
	category_id INT NOT NULL,
	model_year SMALLINT NOT NULL,
	list_price DECIMAL(10, 2) NOT NULL,
	FOREIGN KEY (category_id) REFERENCES categories (category_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (brand_id) REFERENCES brands (brand_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS stocks (
	store_id INT,
	product_id INT,
	quantity INT,
	PRIMARY KEY (store_id, product_id),
	FOREIGN KEY (store_id) REFERENCES sales.stores (store_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- ============================================
-- SALES TABLES (che dipendono da production)
-- ============================================

USE sales;

CREATE TABLE IF NOT EXISTS orders (
	order_id INT AUTO_INCREMENT PRIMARY KEY,
	customer_id INT,
	order_status TINYINT NOT NULL,
	-- Order status: 1 = Pending; 2 = Processing; 3 = Rejected; 4 = Completed
	order_date DATE NOT NULL,
	required_date DATE NOT NULL,
	shipped_date DATE,
	store_id INT NOT NULL,
	staff_id INT NOT NULL,
	FOREIGN KEY (customer_id) REFERENCES customers (customer_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (store_id) REFERENCES stores (store_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (staff_id) REFERENCES staffs (staff_id) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE IF NOT EXISTS order_items (
	order_id INT,
	item_id INT,
	product_id INT NOT NULL,
	quantity INT NOT NULL,
	list_price DECIMAL(10, 2) NOT NULL,
	discount DECIMAL(4, 2) NOT NULL DEFAULT 0,
	PRIMARY KEY (order_id, item_id),
	FOREIGN KEY (order_id) REFERENCES orders (order_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (product_id) REFERENCES production.products (product_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- ============================================
-- STORED PROCEDURES
-- ============================================

DELIMITER //

CREATE PROCEDURE production.uspGetBrandById(IN id INT)
BEGIN
    SELECT brand_id AS id, brand_name AS name
    FROM production.brands
    WHERE brand_id = id;
END //

CREATE PROCEDURE sales.uspGetStoreById(IN id INT)
BEGIN
    SELECT store_id AS id, store_name AS name, phone, email, street, city, state, zip_code AS zipCode
    FROM sales.stores
    WHERE store_id = id;
END //

DELIMITER ;