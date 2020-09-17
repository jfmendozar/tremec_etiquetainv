-- ============================================= 
-- Author:      Francisco Mendoza
-- Create date: <21/06/2018> 
-- Description: Actualiza la estructura de la tabla NumerosParteRaw, ELIMINA campo con valor a actualizar y AGREGAR campo actualizado.
-- History
-- <21-06-2018> <lk.centeno> <Se complementa Script para eliminar constraint que no permite que se duplique la información>
-- ============================================= 

/* Para evitar posibles problemas de pérdida de datos, debe revisar este script detalladamente antes de ejecutarlo fuera del contexto del diseñador de base de datos.*/
BEGIN
  IF EXISTS(SELECT * FROM sys.indexes WHERE Name = 'IX_Codigo_NumerosParteRaw' AND OBJECT_ID = OBJECT_ID(N'NumerosParteRaw'))
    BEGIN
  	  BEGIN TRANSACTION
  	  
  	  ALTER TABLE dbo.NumerosParteRaw
  	  	DROP CONSTRAINT IX_Codigo_NumerosParteRaw

  	  ALTER TABLE dbo.NumerosParteRaw SET (LOCK_ESCALATION = TABLE)
  	  
  	  COMMIT
    END
END
GO

-- Tabla: NumerosParteRaw (Guarda los campos cargados de el proceso de Carga Masiva)
-- Valida si está el campo existe para agregar o eliminar según sea el caso
BEGIN
  -- QUITAR campo
  IF EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'Codigo' AND OBJECT_ID = OBJECT_ID(N'NumerosParteRaw'))
	BEGIN
	  ALTER TABLE NumerosParteRaw
	  DROP COLUMN Codigo;
	END
	-- AGREGAR nuevo campo
  IF NOT EXISTS(SELECT object_id FROM sys.columns WHERE Name = N'Codigo' AND OBJECT_ID = OBJECT_ID(N'NumerosParteRaw'))
	BEGIN
	  ALTER TABLE NumerosParteRaw
	  ADD Codigo VARCHAR(20) NOT NULL;
	END
END
GO

BEGIN
  IF NOT EXISTS(SELECT * FROM sys.indexes WHERE Name = 'IX_Codigo_NumerosParteRaw' AND OBJECT_ID = OBJECT_ID(N'NumerosParteRaw'))
    BEGIN
  	  BEGIN TRANSACTION
  	  
  	  ALTER TABLE dbo.NumerosParteRaw ADD CONSTRAINT
		IX_Codigo_NumerosParteRaw UNIQUE NONCLUSTERED 
		(
		Codigo
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	  ALTER TABLE dbo.NumerosParteRaw SET (LOCK_ESCALATION = TABLE)
  	  
  	  COMMIT
    END
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_001_06_Modifica_Tabla_NumerosParteRaw.sql', 'Script que actualiza el campo Codigo a la tabla NumerosParte.', GETDATE())
GO