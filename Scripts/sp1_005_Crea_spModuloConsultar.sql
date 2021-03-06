IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spModuloConsultar')
	DROP PROCEDURE [dbo].[spModuloConsultar]
GO


-- =============================================
-- Author:		Antonio Gonzalez
-- Create date:	05/06/2017
-- Description:	Consultar modulo
-- History:		
-- <21/08/2017> Antonio Gonzalez - Incluir modulos sin archivo
-- <18/04/2018> lk.centeno: Cambiar la consulta para obtener el nombre y Id de las acciones por modulo 
-- =============================================
CREATE PROCEDURE [dbo].[spModuloConsultar] 
AS
BEGIN

	-- ================================================================
	-- OBTIENE todas las acciones que pueden realizarse en cada modulo
	-- ================================================================
	SELECT IdAccion, Nombre AS Accion
	FROM Accion

	-- ============================================================================
	-- INSERTA en una tabla temporal los modulos con las acciones asociadas a este.
	-- ============================================================================
	SELECT IdModulo
		, CONVERT(bit,(CASE WHEN Agregar IS NOT NULL THEN 1 ELSE 0 END)) AS Agregar
		, CONVERT(bit,(CASE WHEN Consultar IS NOT NULL THEN 1 ELSE 0 END)) AS Consultar
		, CONVERT(bit,(CASE WHEN Editar IS NOT NULL THEN 1 ELSE 0 END)) AS Editar
		, CONVERT(bit,(CASE WHEN Eliminar IS NOT NULL THEN 1 ELSE 0 END)) AS Eliminar
		, CONVERT(bit,(CASE WHEN Exportar IS NOT NULL THEN 1 ELSE 0 END)) AS Exportar
		, CONVERT(bit,(CASE WHEN Importar IS NOT NULL THEN 1 ELSE 0 END)) AS Importar
		--, CONVERT(bit,(CASE WHEN Imprimir IS NOT NULL THEN 1 ELSE 0 END)) AS Imprimir
		--, CONVERT(bit,(CASE WHEN [Cerrar Orden] IS NOT NULL THEN 1 ELSE 0 END)) AS [Cerrar Orden]
	INTO #TMPPERMISOS
	FROM
	(
		SELECT M.IdModulo, M.Nombre, A.IdAccion, A.Nombre AS NombrePermiso
		FROM ModuloAccion MA
		INNER JOIN Modulo M ON M.IdModulo = MA.IdModulo
		INNER JOIN Accion A ON A.IdAccion = MA.IdAccion
	) A 
	PIVOT
	(
		MAX(IdAccion)
		FOR NombrePermiso IN (Agregar, Consultar, Editar, Eliminar, Exportar, Importar)--, Imprimir, [Cerrar Orden])
	) PIV;

	-- ============================================================================
	-- OBTIENE LAS acciones de cada modulo en columnas.
	-- ============================================================================
	SELECT M.IdModulo, M.IdModuloPadre, M.Nombre, M.Archivo, M.Orden, M.RibbonName
		, (CASE WHEN M.TipoModulo = 'W' THEN 'Web' ELSE 'Móvil' END) AS TipoModulo
		, M.FechaActualizado, M.IdUsuarioActualiza, M.Icono, 
		TMP.Agregar, TMP.Consultar, TMP.Editar, TMP.Eliminar, TMP.Exportar, TMP.Importar--, TMP.Imprimir, TMP.[Cerrar Orden]
	FROM Modulo M
	INNER JOIN #TMPPERMISOS TMP ON TMP.IdModulo = M.IdModulo
	WHERE --Archivo IS NOT NULL  AND 
		Archivo IS NOT NULL
	ORDER BY TipoModulo, Nombre ASC

	-- Eliminar tabla temporal
	DROP TABLE #TMPPERMISOS
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_005_Crea_spModuloConsultar.sql', 'Script que Consultar modulo.', GETDATE())
GO