USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Tienda_juegos')
BEGIN
    DROP DATABASE Tienda_juegos;
END
GO

CREATE DATABASE Tienda_juegos;
GO

USE Tienda_juegos;
GO

-- 1. Tabla: Categorias
DROP TABLE IF EXISTS Categorias;
CREATE TABLE Categorias (
    IdCategoria CHAR(6) NOT NULL,
    Descripcion VARCHAR(50) NOT NULL,
    CONSTRAINT PK_Categorias PRIMARY KEY (IdCategoria)
);
GO

INSERT INTO Categorias (IdCategoria, Descripcion) VALUES 
('CAT001', 'Accion'),
('CAT002', 'Aventura'),
('CAT003', 'Casual'),
('CAT004', 'Estrategia'),
('CAT005', 'Rol');
GO

-- 2. Tabla: Juegos (50 registros)
DROP TABLE IF EXISTS Juegos;
CREATE TABLE Juegos (
    IdJuegos INT IDENTITY(1,1) NOT NULL,
    IdCategoria CHAR(6) NOT NULL,
    Descripcion VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Imagen VARCHAR(255) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_Juegos PRIMARY KEY (IdJuegos),
    CONSTRAINT FK_Juegos_Categorias FOREIGN KEY (IdCategoria) REFERENCES Categorias(IdCategoria),
    CONSTRAINT CHK_Juegos_Precio CHECK (Precio > 0)
);
GO

INSERT INTO Juegos(IdCategoria, Descripcion, Precio, Imagen) VALUES
-- Accion
('CAT001','ARC Raiders',111.90,'00001.jpg'),
('CAT001','Clair Obscur Expedition 33',99.90,'00002.jpg'),
('CAT001','HELLDIVERS 2',119.90,'00003.jpg'),
('CAT001','Alien Isolation',39.90,'00004.jpg'),
('CAT001','Hollow Knight Silksong',43.00,'00005.jpg'),
('CAT001','Dragon Ball Xenoverse 2',69.90,'00006.jpg'),
('CAT001','Stellar Blade',139.90,'00007.jpg'),
('CAT001','GTA',49.90,'00008.jpg'),
('CAT001','ELDEN RING NIGHTREIGN',149.90,'00009.jpg'),
('CAT001','Resident Evil 4',45.00,'00010.jpg'),

-- Aventura
('CAT002','Subnautica',89.90,'00011.jpg'),
('CAT002','Grounded 2',129.90,'00012.jpg'),
('CAT002','Far Cry 6',159.90,'00013.jpg'),
('CAT002','Black Myth Wukong',259.90,'00014.jpg'),
('CAT002','Raft',49.90,'00015.jpg'),
('CAT002','Lies of P',179.90,'00016.jpg'),
('CAT002','Kingdom Heart',199.90,'00017.jpg'),
('CAT002','Minecraft',89.90,'00018.jpg'),
('CAT002','Stray',129.90,'00019.jpg'),
('CAT002','BioShock The Collection',89.90,'00020.jpg'),

-- Casual
('CAT003','Forager',14.90,'00021.jpg'),
('CAT003','Coral Island',29.90,'00022.jpg'),
('CAT003','Unpacking',19.90,'00023.jpg'),
('CAT003','Stardew Valley',14.90,'00024.jpg'),
('CAT003','The Sims 4',29.90,'00025.jpg'),
('CAT003','Bloons TD 6',12.90,'00026.jpg'),
('CAT003','Powerwash Simulator',19.90,'00027.jpg'),
('CAT003','Vampire Survivors',9.90,'00028.jpg'),
('CAT003','Tiny Glade',12.90,'00029.jpg'),
('CAT003','Fantasy Life i',39.90,'00030.jpg'),

-- Estrategia
('CAT004','Age of Empires IV',119.90,'00031.jpg'),
('CAT004','Cities Skylines II',159.90,'00032.jpg'),
('CAT004','Balatro',24.90,'00033.jpg'),
('CAT004','Civilization VII',199.90,'00034.jpg'),
('CAT004','Anno 117 Pax Romana',219.90,'00035.jpg'),
('CAT004','Metaphor ReFantazio',259.90,'00036.jpg'),
('CAT004','Shin Megami Tensei V Vengeance',179.90,'00037.jpg'),
('CAT004','Plague Inc Evolved',19.90,'00038.jpg'),
('CAT004','Command and Conquer Remastered',49.90,'00039.jpg'),
('CAT004','Sword of Convallaria',39.90,'00040.jpg'),

-- Rol
('CAT005','Age of Wonders 4',159.90,'00041.jpg'),
('CAT005','Monster Hunter Rise',139.90,'00042.jpg'),
('CAT005','Final Fantasy XIV',79.90,'00043.jpg'),
('CAT005','Yakuza Like a Dragon',129.90,'00044.jpg'),
('CAT005','Hogwarts Legacy',249.90,'00045.jpg'),
('CAT005','Diablo IV',289.90,'00046.jpg'),
('CAT005','Code Vein II',89.90,'00047.jpg'),
('CAT005','Atelier Ryza 3',99.90,'00048.jpg'),
('CAT005','Baldurs Gate 3',129.90,'00049.jpg'),
('CAT005','Darkest Dungeon',29.90,'00050.jpg');
GO

-- 3. Tabla: Roles
DROP TABLE IF EXISTS Roles;
CREATE TABLE Roles (
    IdRol INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(50) NOT NULL UNIQUE,
    CONSTRAINT PK_Roles PRIMARY KEY (IdRol)
);
GO

INSERT INTO Roles (Nombre) VALUES ('ROLE_CLIENTE'), ('ROLE_ADMIN');
GO

-- 4. Tabla: Clientes
DROP TABLE IF EXISTS Clientes;
CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) NOT NULL,
    Apellidos VARCHAR(50) NOT NULL,
    Nombres VARCHAR(50) NOT NULL,
    DNI VARCHAR(12) NOT NULL UNIQUE,
    Direccion VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Sexo CHAR(1) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Estado CHAR(1) NOT NULL DEFAULT '1',
    CONSTRAINT PK_Clientes PRIMARY KEY (IdCliente),
    CONSTRAINT CHK_Clientes_Sexo CHECK (Sexo IN ('M','F')),
    CONSTRAINT CHK_Clientes_Estado CHECK (Estado IN ('0','1'))
);
GO

INSERT INTO Clientes (Apellidos, Nombres, DNI, Direccion, Telefono, FechaNacimiento, Sexo, Correo, Password, Estado) VALUES
('Gonzales', 'Marco Antonio', '45258695', 'Av. Los Olivos 123', '995587456', '1998-05-14', 'M', 'marco.g@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Istrador', 'Admin', '56789423', 'Av. Admin 123', '956894369', '2026-01-01', 'F', 'fakeadmin@gmail.com', '$2a$11$n5yGg4.x05K/0eFwA1d5OuglS53h6sDsmk2.RjUomx4iK2Xg8lBWy', '1'),
('Rojas Mendoza', 'Valeria Sofia', '72145896', 'Av. Brasil 1420, Jesús María', '987123456', '2001-03-22', 'F', 'valeria.rojas@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Flores Castro', 'Diego Alonso', '70852147', 'Jr. Huancavelica 450, Lima', '974581236', '1999-11-05', 'M', 'diego.flores@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Navarro Quispe', 'Camila Andrea', '74563218', 'Av. La Marina 2100, San Miguel', '963258741', '2002-07-19', 'F', 'camila.navarro@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Paredes Silva', 'Carlos Eduardo', '48963254', 'Calle Las Begonias 320, Lince', '951753468', '1995-09-12', 'M', 'carlos.paredes@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Morales Huamán', 'Lucía Fernanda', '76321458', 'Av. Arequipa 3850, Miraflores', '941258963', '2000-12-30', 'F', 'lucia.morales@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Gutiérrez Ramos', 'Sebastián André', '71258963', 'Av. Javier Prado Este 1850, San Borja', '985214796', '1997-04-18', 'M', 'sebastian.gutierrez@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Vargas Medina', 'Ana Patricia', '46852197', 'Jr. Bolognesi 215, Magdalena', '978451236', '1994-08-25', 'F', 'ana.vargas@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1'),
('Castillo Herrera', 'Mateo Ignacio', '75963214', 'Av. Universitaria 1540, Pueblo Libre', '996325874', '2003-01-14', 'M', 'mateo.castillo@gmail.com', '$2a$10$8.UnVuG9HHgffUDAlk8qfOUVGkqRzgVymGe07xd00DMxs.AQy2n9u', '1');
GO

-- 5. Tabla Intermedia: Cliente_Roles
DROP TABLE IF EXISTS Cliente_Roles;
CREATE TABLE Cliente_Roles (
    IdCliente INT NOT NULL,
    IdRol INT NOT NULL,
    CONSTRAINT PK_Cliente_Roles PRIMARY KEY (IdCliente, IdRol),
    CONSTRAINT FK_ClienteRoles_Clientes FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente) ON DELETE CASCADE,
    CONSTRAINT FK_ClienteRoles_Roles FOREIGN KEY (IdRol) REFERENCES Roles(IdRol) ON DELETE CASCADE
);
GO

INSERT INTO Cliente_Roles (IdCliente, IdRol) VALUES 
(1, 1), -- marco.g@gmail.com -> ROLE_CLIENTE
(2, 2), -- admin@gmail.com -> ROLE_ADMIN
(3, 1),
(4, 1),
(5, 1),
(6, 1),
(7, 1),
(8, 1),
(9, 1),
(10, 1);
GO

-- 6. Tabla: Ventas
DROP TABLE IF EXISTS Detalle;
DROP TABLE IF EXISTS Mensajes;
DROP TABLE IF EXISTS Ventas;
CREATE TABLE Ventas (
    IdVenta INT IDENTITY(1,1) NOT NULL,
    IdCliente INT NOT NULL,
    FechaVenta DATETIME NOT NULL DEFAULT GETDATE(),
    MontoTotal DECIMAL(10, 2) NOT NULL,
    Estado CHAR(1) NOT NULL DEFAULT '1',
    CONSTRAINT PK_Ventas PRIMARY KEY (IdVenta),
    CONSTRAINT FK_Ventas_Clientes FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente),
    CONSTRAINT CHK_Ventas_MontoTotal CHECK (MontoTotal > 0),
    CONSTRAINT CHK_Ventas_Estado CHECK (Estado IN ('0', '1'))
);
GO

-- 7. Tabla: Detalle
CREATE TABLE Detalle (
    IdVenta INT NOT NULL,
    IdJuegos INT NOT NULL,
    Cantidad INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Estado CHAR(1) NOT NULL DEFAULT '1',
    CONSTRAINT PK_Detalle PRIMARY KEY (IdVenta, IdJuegos),
    CONSTRAINT FK_Detalle_Ventas FOREIGN KEY (IdVenta) REFERENCES Ventas(IdVenta),
    CONSTRAINT FK_Detalle_Juegos FOREIGN KEY (IdJuegos) REFERENCES Juegos(IdJuegos),
    CONSTRAINT CHK_Detalle_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CHK_Detalle_Precio CHECK (Precio > 0),
    CONSTRAINT CHK_Detalle_Estado CHECK (Estado IN ('0','1'))
);
GO

-- 8. Tabla: Mensajes
CREATE TABLE Mensajes (
    IdMensaje INT IDENTITY(1,1) NOT NULL, 
    IdCliente INT NOT NULL,
    TextoMensaje VARCHAR(250) NOT NULL,
    FechaEnvio DATETIME NOT NULL DEFAULT GETDATE(),
    Estado CHAR(1) NOT NULL DEFAULT '1',
    CONSTRAINT PK_Mensajes PRIMARY KEY (IdMensaje),
    CONSTRAINT FK_Mensajes_Clientes FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente)
);
GO

INSERT INTO Mensajes (IdCliente, TextoMensaje, FechaEnvio, Estado) VALUES
(1, 'Consulta sobre stock disponible', '2026-06-15 10:30:00', '1');
GO