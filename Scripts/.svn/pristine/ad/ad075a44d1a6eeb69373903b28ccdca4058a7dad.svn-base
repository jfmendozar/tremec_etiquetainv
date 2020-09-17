-- ============================================= 
-- Author:      Laura K. Centeno Ch.
-- Create date: <26/06/2018> 
-- Description: Actualiza la estructura de la tabla Produccion, AGREGAR campo de Lote
-- ============================================= 

-- Tabla: CodigosIdentificacion
-- Valida si el campo existe para agregar o eliminar según sea el caso
BEGIN
  -- AGREGAR nuevo campo Lote del Numero de Parte
  IF NOT EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'Name' AND OBJECT_ID = OBJECT_ID(N'CodigosIdentificacion'))
	BEGIN
	  ALTER TABLE CodigosIdentificacion
	  ADD Name VARCHAR(50) NULL;
	END
  IF EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'IC' AND OBJECT_ID = OBJECT_ID(N'CodigosIdentificacion'))
	BEGIN
	  ALTER TABLE CodigosIdentificacion
	  DROP COLUMN IC;

	  ALTER TABLE CodigosIdentificacion
	  ADD IC VARCHAR(10) NULL; 
	END
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_001_08_Modifica_Tabla_CodigosIdentificacion.sql', 'Script que agrega el campo Name y el campo IC a la tabla CodigosIdentificacion.', GETDATE())
GO