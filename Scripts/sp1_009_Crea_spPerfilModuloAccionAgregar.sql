IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spPerfilModuloAccionAgregar')
	DROP PROCEDURE [dbo].[spPerfilModuloAccionAgregar]
GO


-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  07/06/2017
-- Description:	 Agregar perfil modulo accion
-- History:
-- <23/04/2018> <lk.centeno> <Modificar script para recibir todos los permisos asignados al perfil en una sola ejecución.>
-- <13-06-2018> <lk.centeno> <Eliminar permisos que no se usaran en este proyecto.>
-- =============================================
CREATE PROCEDURE [dbo].[spPerfilModuloAccionAgregar]
@pIdPerfil	INT,
@pNombre	VARCHAR(100),
@pClave		VARCHAR(30),
@pModuloPermisos VARCHAR(MAX),
@pIdUsuarioActualiza INT
AS

BEGIN
  BEGIN TRY
	BEGIN TRAN
	
	-- ============================================= --
	-- DEFINICION de variables
	-- ============================================= --
	DECLARE
		-- Variables para el manejo de errores
		@vIdUsuario INT,
		-- Variables para el cursor
		@vIdModulo INT, @vAgregar INT, @vConsultar INT, @vEditar INT, @vEliminar INT, @vExportar INT,
		@vImportar INT,-- @vImprimir INT, @vCerrarOrden INT,
		-- Variables para guardar el ID de la accion
		@vIdAccionAgregar INT, @vIdAccionConsultar INT, @vIdAccionEditar INT, @vIdAccionEliminar INT,
		@vIdAccionExportar INT, @vIdAccionImportar INT, @vIdAccionImprimir INT, @vIdAccionCerrarOrden INT,
		@vMenosPermisos INT = 0, @vMasPermisos INT = 0, @vPerfilPersonalizado BIT,
		@vDelimiterInit VARCHAR(4) = '|', @vDelimiter VARCHAR(1) = ','
	-- Tabla temporal para guardar cada modulo con sus respectivos permisos
	DECLARE @temptable TABLE (ModuloPermisos varchar(200))
	DECLARE @tempPermisos TABLE (IdModulo INT, Agregar BIT, Consultar BIT, Editar BIT, Eliminar BIT, Exportar BIT, 
								Importar BIT)--, Imprimir BIT, [Cerrar Orden] BIT)
	DECLARE @tempResultado TABLE (Resultado VARCHAR(20))

	
	-- ============================================= --
	-- AGREGAR/ACTUALIZAR USUARIO
	-- ============================================= --
	IF @pIdPerfil > 0
	  BEGIN
		INSERT INTO @tempResultado
		EXEC spPerfilEditar @pIdPerfil, @pNombre, @pClave, @pIdUsuarioActualiza
	  END
	ELSE
	  BEGIN
		INSERT INTO @tempResultado
		EXEC spPerfilAgregar @pNombre, @pClave, @pIdUsuarioActualiza, @pIdPerfil OUTPUT
	  END

	-- ============================================= --
	-- VALIDAR resultado de AGREGAR/ACTUALIZAR usuario
	-- ============================================= --
	IF ((SELECT Resultado FROM @tempResultado) <> 'OK')
	  BEGIN
		SELECT Resultado FROM @tempResultado
		COMMIT TRAN
		RETURN
	  END

	-- ============================================= --
	-- OBTENER el Id correspondiente a la acción
	-- ============================================= --
	SELECT @vIdAccionAgregar = IdAccion FROM Accion WHERE Nombre = 'Agregar'
	SELECT @vIdAccionConsultar = IdAccion FROM Accion WHERE Nombre = 'Consultar'
	SELECT @vIdAccionEditar = IdAccion FROM Accion WHERE Nombre = 'Editar'
	SELECT @vIdAccionEliminar = IdAccion FROM Accion WHERE Nombre = 'Eliminar'
	SELECT @vIdAccionExportar = IdAccion FROM Accion WHERE Nombre = 'Exportar'
	SELECT @vIdAccionImportar = IdAccion FROM Accion WHERE Nombre = 'Importar'
	--SELECT @vIdAccionImprimir = IdAccion FROM Accion WHERE Nombre = 'Imprimir'
	--SELECT @vIdAccionCerrarOrden = IdAccion FROM Accion WHERE Nombre = 'Cerrar Orden'

	-- ============================================= --
	-- OBTENER los módulos y los permisos que se tiene asignado el perfil en cada módulo
	-- ============================================= --
	INSERT INTO @temptable 
	SELECT Split.a.value('.', 'VARCHAR(200)') AS ModuloPermisos
	FROM (
	    SELECT CAST ('<M>' + REPLACE(@pModuloPermisos, @vDelimiterInit, '</M><M>') + '</M>' AS XML) AS CVS
	) AS A
	CROSS APPLY CVS.nodes ('/M') AS Split(a)
	
	-- FORMA una tabla con los registros
	;WITH XMLTable (xmlTag)
	AS( SELECT CONVERT(XML,'<CSV><champ>' + REPLACE(ModuloPermisos, @vDelimiter, '</champ><champ>') + '</champ></CSV>') AS xmlTag
		FROM @temptable)
	-- AGREGA los valores a la tabla temporal
	INSERT INTO @tempPermisos (IdModulo, Agregar, Consultar, Editar, Eliminar, Exportar, Importar)
	SELECT RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[1]','int'))) AS IdModulo,
	       RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[2]','bit'))) AS Agregar,
	       RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[3]','bit'))) AS Consultar,   
	       RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[4]','bit'))) AS Editar,
		   RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[5]','bit'))) AS Eliminar,
		   RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[6]','bit'))) AS Exportar,
		   RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[7]','bit'))) AS Importar
		   --RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[8]','bit'))) AS Imprimir,
		   --RTRIM(LTRIM(xmlTag.value('/CSV[1]/champ[9]','bit'))) AS [Cerrar Orden]
	FROM XMLTable

	-- ============================================= --
	-- ELIMINAR los permisos que tiene asignados el Perfil
	-- ============================================= --
	DELETE FROM PerfilModuloAccion WHERE IdPerfil = @pIdPerfil --AND IdModulo = @pIdModulo
	
	-- ============================================= --
	-- DEFINE CURSOR cursorModulos: Obtiene los datos de 
	-- ============================================= --
	DECLARE cursorModulos CURSOR FOR 
	SELECT IdModulo, Agregar, Consultar, Editar, Eliminar, Exportar, Importar--, Imprimir, [Cerrar Orden]
	FROM @tempPermisos
	
	-- ABRIR el cursor cursorModulos
	OPEN cursorModulos
	FETCH NEXT FROM cursorModulos INTO @vIdModulo, @vAgregar, @vConsultar, @vEditar, @vEliminar, @vExportar, @vImportar--, @vImprimir, @vCerrarOrden
	WHILE @@FETCH_STATUS = 0
	  BEGIN

		IF @vAgregar = 1
		  BEGIN
			INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
			VALUES (@pIdPerfil, @vIdModulo, @vIdAccionAgregar, GETDATE(), @pIdUsuarioActualiza);
		  END
		IF @vConsultar = 1
		  BEGIN
			INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
			VALUES (@pIdPerfil, @vIdModulo, @vIdAccionConsultar, GETDATE(), @pIdUsuarioActualiza);
		  END
		IF @vEditar = 1
		  BEGIN
			INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
			VALUES (@pIdPerfil, @vIdModulo, @vIdAccionEditar, GETDATE(), @pIdUsuarioActualiza);
		  END
		IF @vEliminar = 1
		  BEGIN
			INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
			VALUES (@pIdPerfil, @vIdModulo, @vIdAccionEliminar, GETDATE(), @pIdUsuarioActualiza);
		  END
		IF @vExportar = 1
		  BEGIN
			INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
			VALUES (@pIdPerfil, @vIdModulo, @vIdAccionExportar, GETDATE(), @pIdUsuarioActualiza);
		  END
		IF @vImportar = 1
		  BEGIN
			INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
			VALUES (@pIdPerfil, @vIdModulo, @vIdAccionImportar, GETDATE(), @pIdUsuarioActualiza);
		  END
		--IF @vImprimir = 1
		--  BEGIN
		--	INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
		--	VALUES (@pIdPerfil, @vIdModulo, @vIdAccionImprimir, GETDATE(), @pIdUsuarioActualiza);
		--  END
		--IF @vCerrarOrden = 1
		--  BEGIN
		--	INSERT INTO PerfilModuloAccion(IdPerfil, IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
		--	VALUES (@pIdPerfil, @vIdModulo, @vIdAccionCerrarOrden, GETDATE(), @pIdUsuarioActualiza);
		--  END

		-- OBTIENE el siguiente registro del CURSOR cursorModulos
		FETCH NEXT FROM cursorModulos INTO @vIdModulo, @vAgregar, @vConsultar, @vEditar, @vEliminar, @vExportar, @vImportar--, @vImprimir, @vCerrarOrden

	  END
	-- CERRAR el cursor cursorModulos
	CLOSE cursorModulos
	-- QUITAR referencia al cursor
	DEALLOCATE cursorModulos

	-- ============================================= --
	-- AGREGAR/ACTUALIZAR USUARIO
	-- ============================================= --
	EXEC spPermisoAgregar @pIdPerfil, @pIdUsuarioActualiza

	-- Si todo fue correcto se envia la respuesta de OK
	SELECT 'OK' AS Resultado

	COMMIT TRAN
  END TRY
  BEGIN CATCH
	-- Si existe algun problema revertir todas las transacciones
	IF @@TRANCOUNT > 0
		ROLLBACK TRAN;
	-- Declarar variables para el manejo del error
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Agregar perfil modulo accion.'
	SET @vIncidente = 'Error al insertar datos en la tabla PerfilModuloAccion.'
	SET @vEsError = 1
	-- Agregar el log a la tabla de movimientos.
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
	-- Devuelve el mensaje de error
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_009_Crea_spPerfilModuloAccionAgregar.sql', 'Script que asocia un perfil con los modulos y acciones.', GETDATE())
GO