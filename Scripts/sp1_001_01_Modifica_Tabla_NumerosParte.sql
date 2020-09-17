-- ============================================= 
-- Author:      Laura K. Centeno Ch.
-- Create date: <30/05/2018> 
-- Description: Actualiza la estructura de la tabla NumerosParte, AGREGAR nuevo campo requerido y ELIMINAR campos que no se requieren.
-- ============================================= 

-- Tabla: NumerosParte (Guarda los detalles del Número de Parte)
-- Valida si está el campo existe para agregar o eliminar según sea el caso
BEGIN
  -- AGREGAR nuevo campo
  IF NOT EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'Descripcion' AND OBJECT_ID = OBJECT_ID(N'NumerosParte'))
	BEGIN
	  ALTER TABLE NumerosParte
	  ADD Descripcion VARCHAR(500) NULL;
	END

  -- QUITAR campo
  IF EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'Lote' AND OBJECT_ID = OBJECT_ID(N'NumerosParte'))
	BEGIN
	  ALTER TABLE NumerosParte
	  DROP COLUMN Lote;
	END

  -- QUITAR campo
  IF EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'NumSerie' AND OBJECT_ID = OBJECT_ID(N'NumerosParte'))
	BEGIN
	  ALTER TABLE NumerosParte
	  DROP COLUMN NumSerie;
	END

  -- QUITAR campo
  IF EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'CantidadTotal' AND OBJECT_ID = OBJECT_ID(N'NumerosParte'))
	BEGIN
	  ALTER TABLE NumerosParte
	  DROP COLUMN CantidadTotal;
	END

  -- QUITAR campo
  IF EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'FechaFabricacion' AND OBJECT_ID = OBJECT_ID(N'NumerosParte'))
	BEGIN
	  ALTER TABLE NumerosParte
	  DROP COLUMN FechaFabricacion;
	END
END
GO
-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_001_01_Modifica_Tabla_NumerosParte.sql', 'Script que agrega el campo Descripcion a la tabla NumerosParte.', GETDATE())
GO