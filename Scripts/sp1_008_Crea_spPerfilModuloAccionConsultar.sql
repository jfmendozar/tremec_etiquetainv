IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spPerfilModuloAccionConsultar')
	DROP PROCEDURE [dbo].[spPerfilModuloAccionConsultar]
GO
-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  05/06/2017
-- Description:	 Consultar los permisos que tiene cada modulo asignado a un perfil
-- History:
-- <23/04/2018> <lk.centeno> <Se incluyen las columnas de Importar, Imprimir, Cerrar Orden; en la consulta.>
-- =============================================
CREATE PROCEDURE [dbo].[spPerfilModuloAccionConsultar] 
@pIdPerfil INT

AS
BEGIN
  BEGIN TRY

	-- =============================================
	-- OBTENER Los Modulos y sus acciones
	-- =============================================
	SELECT P.IdPerfil, M.IdModulo, A.IdAccion, A.Nombre AS NombrePermiso, M.Nombre AS NombreModulo, M.TipoModulo
	INTO #PERMISOS_TMP
	FROM PerfilModuloAccion P
	INNER JOIN Modulo M ON M.IdModulo = P.IdModulo 
	INNER JOIN Accion A ON P.IdAccion = A.IdAccion
	WHERE P.IdPerfil = @pIdPerfil

	-- ===========================================================
	-- FORMATEAR consulta para mostrar sus acciones como columnas
	-- ===========================================================
	SELECT IdPerfil, IdModulo, NombreModulo, TipoModulo
		, (CASE WHEN Agregar IS NOT NULL THEN 1 ELSE 0 END) AS Agregar
		, (CASE WHEN Consultar IS NOT NULL THEN 1 ELSE 0 END) AS Consultar
		, (CASE WHEN Editar IS NOT NULL THEN 1 ELSE 0 END) AS Editar
		, (CASE WHEN Eliminar IS NOT NULL THEN 1 ELSE 0 END) AS Eliminar
		, (CASE WHEN Exportar IS NOT NULL THEN 1 ELSE 0 END) AS Exportar
		, (CASE WHEN Importar IS NOT NULL THEN 1 ELSE 0 END) AS Importar
		--, (CASE WHEN Imprimir IS NOT NULL THEN 1 ELSE 0 END) AS Imprimir
		--, (CASE WHEN [Cerrar Orden] IS NOT NULL THEN 1 ELSE 0 END) AS [Cerrar Orden]
	FROM
	(
		SELECT DISTINCT TMP.IdPerfil, M.IdModulo, A.IdAccion, A.Nombre AS NombrePermiso, M.Nombre AS NombreModulo
			, M.IdModuloPadre, M.Archivo, M.Orden, M.RibbonName, M.Icono, M.TipoModulo
		FROM #PERMISOS_TMP TMP 
		INNER JOIN Modulo M ON  M.IdModulo = TMP.IdModulo
		INNER JOIN Accion A ON A.IdAccion = TMP.IdAccion
	) A 
	PIVOT
	(
		MAX(IdAccion)
		FOR NombrePermiso IN (Agregar, Consultar, Editar, Eliminar, Exportar, Importar)--, Imprimir, [Cerrar Orden])
	) PIV;

	-- =============================================
	-- ELIMINAR tabla temporal
	-- =============================================
	DROP TABLE #PERMISOS_TMP

  END TRY
  BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN;
		-- Variables para el manejo de errores
		DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
		SET @ErrMsg = ERROR_MESSAGE()
		SET @ErrSev = ERROR_SEVERITY()
		SET @ErrSt = ERROR_STATE()		
		SET @vFuncionalidad = 'Agregar perfil modulo accion.'
		SET @vIncidente = 'Error al insertar datos en la tabla PerfilModuloAccion.'
		SET @vEsError = 1
		
		EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, NULL
		RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
	END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_008_Crea_spPerfilModuloAccionConsultar.sql', 'Script que consulta los permisos que tiene cada modulo asignado a un perfil.', GETDATE())
GO