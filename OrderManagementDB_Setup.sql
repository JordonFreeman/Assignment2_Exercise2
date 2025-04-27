CREATE DATABASE OrderManagementDB;
GO

USE OrderManagementDB;
GO

-- Create Tables
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    IsLocked BIT NOT NULL DEFAULT 0,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User',
    LastLogin DATETIME NULL
);

CREATE TABLE Item (
    ItemID INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(100) NOT NULL,
    Size NVARCHAR(50) NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    Description NVARCHAR(500) NULL
);

CREATE TABLE Agent (
    AgentID INT IDENTITY(1,1) PRIMARY KEY,
    AgentName NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    ContactNumber NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL
);

CREATE TABLE [Order] (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME NOT NULL,
    AgentID INT NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Order_Agent FOREIGN KEY (AgentID) REFERENCES Agent(AgentID)
);

CREATE TABLE OrderDetail (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ItemID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitAmount DECIMAL(18, 2) NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_OrderDetail_Item FOREIGN KEY (ItemID) REFERENCES Item(ItemID)
);

-- Insert Sample Data

-- Users (at least 15 rows)
INSERT INTO Users (UserName, Email, Password, IsLocked, Role, LastLogin)
VALUES
('admin', 'admin@example.com', 'admin123', 0, 'Admin', GETDATE()),
('user1', 'user1@example.com', 'user123', 0, 'User', GETDATE()),
('user2', 'user2@example.com', 'user456', 0, 'User', GETDATE()),
('manager', 'manager@example.com', 'manager123', 0, 'Manager', GETDATE()),
('sales', 'sales@example.com', 'sales123', 0, 'Sales', GETDATE()),
('user3', 'user3@example.com', 'user789', 0, 'User', GETDATE()),
('user4', 'user4@example.com', 'user101', 0, 'User', GETDATE()),
('supervisor', 'supervisor@example.com', 'super123', 0, 'Manager', GETDATE()),
('analyst', 'analyst@example.com', 'analyst123', 0, 'Analyst', GETDATE()),
('tech', 'tech@example.com', 'tech123', 0, 'Support', GETDATE()),
('finance', 'finance@example.com', 'finance123', 0, 'Finance', GETDATE()),
('hr', 'hr@example.com', 'hr123', 0, 'HR', GETDATE()),
('marketing', 'marketing@example.com', 'marketing123', 0, 'Marketing', GETDATE()),
('warehouse', 'warehouse@example.com', 'warehouse123', 0, 'Warehouse', GETDATE()),
('logistics', 'logistics@example.com', 'logistics123', 0, 'Logistics', GETDATE());

-- Items (at least 15 rows)
INSERT INTO Item (ItemName, Size, UnitPrice, StockQuantity, Description)
VALUES
('Laptop', '15-inch', 1200.00, 50, 'High-performance laptop with Intel Core i7'),
('Smartphone', '6.1-inch', 800.00, 100, 'Latest model with 128GB storage'),
('Tablet', '10-inch', 500.00, 75, 'Android tablet with 64GB storage'),
('Headphones', 'Over-ear', 150.00, 200, 'Noise-cancelling wireless headphones'),
('Monitor', '27-inch', 350.00, 60, '4K UHD display with HDR'),
('Keyboard', 'Full-size', 80.00, 120, 'Mechanical gaming keyboard'),
('Mouse', 'Standard', 50.00, 150, 'Wireless ergonomic mouse'),
('Printer', 'Desktop', 250.00, 40, 'Color laser printer'),
('External HDD', '2TB', 120.00, 80, 'Portable external hard drive'),
('Smart Watch', '44mm', 300.00, 65, 'Fitness tracker with heart rate monitor'),
('Camera', 'DSLR', 900.00, 30, 'Professional camera with 24MP sensor'),
('Speakers', 'Bookshelf', 200.00, 45, 'Bluetooth speakers with deep bass'),
('Router', 'Dual-band', 120.00, 55, 'High-speed WiFi 6 router'),
('Game Console', 'Standard', 500.00, 25, 'Latest gaming console with controller'),
('Graphics Card', 'High-end', 800.00, 20, 'Gaming graphics card with 12GB VRAM'),
('Power Bank', '20000mAh', 60.00, 90, 'High-capacity portable charger'),
('Webcam', 'HD', 80.00, 70, '1080p webcam with built-in microphone'),
('USB Hub', '4-port', 30.00, 100, 'USB 3.0 hub with 4 ports'),
('SSD', '1TB', 150.00, 60, 'Solid-state drive for fast storage'),
('Network Switch', '8-port', 70.00, 40, 'Gigabit Ethernet switch');

-- Agents (at least 15 rows)
INSERT INTO Agent (AgentName, Address, ContactNumber, Email)
VALUES
('Tech Solutions Inc.', '123 Main St, New York, NY 10001', '212-555-1234', 'sales@techsolutions.com'),
('Digital Distributors', '456 Oak Ave, Los Angeles, CA 90001', '323-555-5678', 'info@digitaldist.com'),
('Global Gadgets', '789 Pine Rd, Chicago, IL 60007', '312-555-9012', 'orders@globalgadgets.com'),
('Electronics Wholesale', '101 Maple Dr, Houston, TX 77001', '281-555-3456', 'wholesale@electronics.com'),
('Tech Imports Ltd.', '202 Cedar Ln, Miami, FL 33101', '305-555-7890', 'imports@techimports.com'),
('Gadget Galaxy', '303 Elm St, Seattle, WA 98101', '206-555-1234', 'sales@gadgetgalaxy.com'),
('Computer Connections', '404 Birch Ave, Denver, CO 80201', '303-555-5678', 'orders@computerconn.com'),
('Smart Tech Supplies', '505 Walnut Rd, Phoenix, AZ 85001', '602-555-9012', 'supplies@smarttech.com'),
('Digital Devices Inc.', '606 Cherry Dr, Philadelphia, PA 19101', '215-555-3456', 'devices@digitalinc.com'),
('Tech Traders', '707 Spruce Ln, Atlanta, GA 30301', '404-555-7890', 'trade@techtraders.com'),
('Electronics Express', '808 Fir St, Boston, MA 02101', '617-555-1234', 'express@electronics.com'),
('Future Tech', '909 Aspen Ave, San Francisco, CA 94101', '415-555-5678', 'future@futuretech.com'),
('Gadget Guys', '111 Redwood Rd, Portland, OR 97201', '503-555-9012', 'guys@gadgets.com'),
('Computer Corner', '222 Sequoia Dr, Dallas, TX 75201', '214-555-3456', 'corner@computers.com'),
('Tech Trends', '333 Willow Ln, San Diego, CA 92101', '619-555-7890', 'trends@techtrends.com'),
('IT Supplies Co.', '444 Laurel Dr, Austin, TX 78701', '512-555-1234', 'info@itsupplies.com'),
('Electronic Planet', '555 Magnolia Ave, Nashville, TN 37201', '615-555-5678', 'sales@eplanet.com'),
('Digital World', '666 Juniper Rd, Las Vegas, NV 89101', '702-555-9012', 'world@digital.com'),
('Tech Warehouse', '777 Cypress Ln, Orlando, FL 32801', '407-555-3456', 'warehouse@tech.com'),
('Gadget Outlet', '888 Poplar St, Minneapolis, MN 55401', '612-555-7890', 'outlet@gadgets.com');

-- Orders and Order Details (at least 15 rows)
-- Order 1
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-01-15', 1, 6200.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(1, 1, 3, 1200.00, 3600.00),
(1, 4, 5, 150.00, 750.00),
(1, 7, 10, 50.00, 500.00),
(1, 10, 3, 300.00, 900.00),
(1, 13, 3, 120.00, 360.00);

-- Order 2
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-02-20', 3, 8500.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(2, 2, 5, 800.00, 4000.00),
(2, 5, 6, 350.00, 2100.00),
(2, 11, 2, 900.00, 1800.00);

-- Order 3
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-03-10', 2, 12000.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(3, 14, 10, 500.00, 5000.00),
(3, 15, 5, 800.00, 4000.00),
(3, 3, 6, 500.00, 3000.00);

-- Order 4
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-04-05', 5, 4350.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(4, 8, 10, 250.00, 2500.00),
(4, 9, 10, 120.00, 1200.00),
(4, 13, 5, 120.00, 600.00);

-- Order 5
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-05-18', 4, 9900.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(5, 1, 5, 1200.00, 6000.00),
(5, 6, 30, 80.00, 2400.00),
(5, 12, 5, 200.00, 1000.00);

-- Order 6
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-06-22', 7, 5000.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(6, 3, 10, 500.00, 5000.00);

-- Order 7
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-07-10', 9, 7000.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(7, 2, 5, 800.00, 4000.00),
(7, 10, 10, 300.00, 3000.00);

-- Order 8
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-08-05', 11, 6500.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(8, 5, 10, 350.00, 3500.00),
(8, 11, 3, 900.00, 2700.00),
(8, 12, 1, 200.00, 200.00);

-- Order 9
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-09-15', 13, 8200.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(9, 4, 20, 150.00, 3000.00),
(9, 14, 5, 500.00, 2500.00),
(9, 7, 40, 50.00, 2000.00),
(9, 8, 2, 250.00, 500.00);

-- Order 10
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-10-28', 6, 5700.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(10, 3, 5, 500.00, 2500.00),
(10, 2, 4, 800.00, 3200.00);

-- Order 11
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-11-05', 8, 3950.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(11, 16, 20, 60.00, 1200.00),
(11, 17, 15, 80.00, 1200.00),
(11, 18, 50, 30.00, 1500.00);

-- Order 12
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2023-12-12', 10, 10500.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(12, 19, 20, 150.00, 3000.00),
(12, 1, 5, 1200.00, 6000.00),
(12, 20, 15, 70.00, 1050.00);

-- Order 13
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2024-01-08', 12, 8500.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(13, 2, 10, 800.00, 8000.00),
(13, 18, 15, 30.00, 450.00);

-- Order 14
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2024-02-20', 14, 7250.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(14, 4, 15, 150.00, 2250.00),
(14, 11, 4, 900.00, 3600.00),
(14, 7, 20, 50.00, 1000.00);

-- Order 15
INSERT INTO [Order] (OrderDate, AgentID, TotalAmount, Status)
VALUES ('2024-03-15', 15, 9150.00, 'Completed');

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount, TotalAmount)
VALUES 
(15, 5, 10, 350.00, 3500.00),
(15, 10, 5, 300.00, 1500.00),
(15, 14, 8, 500.00, 4000.00);

-- Create indexes for better performance
CREATE INDEX IX_Item_ItemName ON Item(ItemName);
CREATE INDEX IX_Agent_AgentName ON Agent(AgentName);
CREATE INDEX IX_Order_OrderDate ON [Order](OrderDate);
CREATE INDEX IX_Order_AgentID ON [Order](AgentID);
CREATE INDEX IX_OrderDetail_OrderID ON OrderDetail(OrderID);
CREATE INDEX IX_OrderDetail_ItemID ON OrderDetail(ItemID);

-- Additional queries to help with reporting
-- Query to find best selling items by quantity
GO
CREATE VIEW vw_BestSellingItems AS
SELECT 
    i.ItemID,
    i.ItemName,
    i.Size,
    i.UnitPrice,
    i.StockQuantity,
    SUM(od.Quantity) AS TotalQuantitySold,
    SUM(od.TotalAmount) AS TotalRevenue
FROM 
    Item i
JOIN 
    OrderDetail od ON i.ItemID = od.ItemID
GROUP BY 
    i.ItemID, i.ItemName, i.Size, i.UnitPrice, i.StockQuantity
GO

-- Query to find items purchased by a specific agent
CREATE PROCEDURE sp_GetItemsPurchasedByAgent
    @AgentID INT
AS
BEGIN
    SELECT DISTINCT
        i.ItemID,
        i.ItemName,
        i.Size,
        i.UnitPrice,
        i.StockQuantity,
        i.Description
    FROM 
        Item i
    JOIN 
        OrderDetail od ON i.ItemID = od.ItemID
    JOIN 
        [Order] o ON od.OrderID = o.OrderID
    WHERE 
        o.AgentID = @AgentID
END
GO

-- Query to find agents who ordered a specific item
CREATE PROCEDURE sp_GetAgentsWhoOrderedItem
    @ItemID INT
AS
BEGIN
    SELECT DISTINCT
        a.AgentID,
        a.AgentName,
        a.Address,
        a.ContactNumber,
        a.Email
    FROM 
        Agent a
    JOIN 
        [Order] o ON a.AgentID = o.AgentID
    JOIN 
        OrderDetail od ON o.OrderID = od.OrderID
    WHERE 
        od.ItemID = @ItemID
END
GO

-- Query to get complete order details for printing
CREATE PROCEDURE sp_GetOrderDetails
    @OrderID INT
AS
BEGIN
    -- Get order information
    SELECT 
        o.OrderID,
        o.OrderDate,
        o.TotalAmount,
        o.Status,
        a.AgentID,
        a.AgentName,
        a.Address,
        a.ContactNumber,
        a.Email
    FROM 
        [Order] o
    JOIN 
        Agent a ON o.AgentID = a.AgentID
    WHERE 
        o.OrderID = @OrderID

    -- Get order details
    SELECT 
        od.ID,
        od.ItemID,
        i.ItemName,
        i.Size,
        od.Quantity,
        od.UnitAmount,
        od.TotalAmount
    FROM 
        OrderDetail od
    JOIN 
        Item i ON od.ItemID = i.ItemID
    WHERE 
        od.OrderID = @OrderID
END
GO