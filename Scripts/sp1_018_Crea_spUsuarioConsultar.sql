IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spUsuarioConsultar')
	DROP PROCEDURE [dbo].[spUsuarioConsultar]
GO
-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  02/06/2017
-- Description:	 Consultar usuarios
-- History:
-- <23/04/2018> lk.centeno: se agrega el valor "(personalizado)" al perfil, en caso de que así sea
-- <21/05/2018> ja.torner: Se filtra el usuario System para que no se modifique en web.
-- History:		06/07/2018 - Antonio Gonzalez - Consulta campo PerfilPersonalizado como tal
-- =============================================
CREATE PROCEDURE [dbo].[spUsuarioConsultar]
AS
BEGIN
  BEGIN TRY

	-- OBTENER los datos de todos los usuarios registrados con estado ACTIVO
	SELECT U.IdUsuario, U.Usuario, U.Nombre, P.IdPerfil, T.IdTipoUsuario, T.Nombre AS TipoUsuario, U.PerfilPersonalizado 
	FROM Usuario U 
	INNER JOIN Perfil P ON U.IdPerfil = P.IdPerfil
	INNER JOIN TipoUsuario T ON U.IDTIPOUSUARIO = T.IdTipoUsuario
	WHERE U.Activo = 1 AND U.IdUsuario <> 1
	ORDER BY Usuario ASC

  END TRY
  BEGIN CATCH
	-- DECLARACION de variables
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Editar usuario.'
	SET @vIncidente = 'Error al actualziar datos en la tabla Usuario.'
	SET @vEsError = 1
	-- GUARADAR error en el log de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, NULL
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_018_Crea_spUsuarioConsultar.sql', 'Script que edita los valores de un usuario especifico.', GETDATE())
GO