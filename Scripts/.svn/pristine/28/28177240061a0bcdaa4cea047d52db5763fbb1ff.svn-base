-- ============================================= 
-- Author:      Laura K. Centeno Ch.
-- Create date: <30/05/2018> 
-- Description: Actualiza la estructura de la tabla Produccion, AGREGAR campo de Lote
-- History:
-- <13/06/2018> <jf.mendoza> <Actualizar el tipo de dato de TipoEtiqueta>
-- ============================================= 

-- Tabla: Produccion (Guarda los movimientos del material, Cuando se agrega el material a un Bin de Proveedor o a un Bin de TREMEC)
-- Valida si el campo existe para agregar o eliminar según sea el caso
BEGIN
  -- AGREGAR nuevo campo Lote del Numero de Parte
  IF NOT EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'Lote' AND OBJECT_ID = OBJECT_ID(N'Produccion'))
	BEGIN
	  ALTER TABLE Produccion
	  ADD Lote VARCHAR(30) NULL;
	END
  IF EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'TipoEtiqueta' AND OBJECT_ID = OBJECT_ID(N'Produccion'))
	BEGIN
	  ALTER TABLE Produccion
	  DROP COLUMN TipoEtiqueta;

	  ALTER TABLE Produccion
	  ADD TipoEtiqueta VARCHAR(30) NULL; 
	END
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp_001_02_Modifica_Tabla_Produccion.sql', 'Script que agrega el campo Lote a la tabla de Produccion.', GETDATE())
GO