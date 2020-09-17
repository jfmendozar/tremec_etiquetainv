IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spNumerosParteObtener')
	DROP PROCEDURE [dbo].[spNumerosParteObtener]
GO
-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 04/06/2018
-- Description:	Script que valida si el Número de Parte existen la tabla NumerosParte
-- History
-- <13/06/2018> <jf.mendoza> <Se agrego el parametro IdUsuarioActualiza y el valor Resultado en el SELECT>
-- <15/06/2018> <jf.mendoza> <Se agrego la condición else solo para validar cuando no existe el número de parte>
-- <26-06-2018> <lk.centeno> <Se cambia la validación del Numero de Parte y se devuelve el NumParte como Resulado>
-- <26-07-2018> <lk.centeno> <Se cambia la validación del Numero de Parte que coincida sin importar el el texto a la derecha>
-- <22-03-2019> <jf.mendoza> <Se agrega consulta para obtener el Numero de Parte exacto, en caso de no encontrar uno, consulta como ya se estaba trabajando.>
-- =============================================
CREATE PROCEDURE [dbo].[spNumerosParteObtener]
@pNumParte VARCHAR(30),
@pIdUsuarioActualiza INT

AS
BEGIN
  BEGIN TRY
	-- DEFINIR variables de uso
	DECLARE @vNumParte VARCHAR(20) = NULL

	-- OBTENER Numeros de Parte
	SET @vNumParte = (SELECT NumParte FROM NumerosParte 
						WHERE UPPER(@pNumParte) = UPPER(NumParte))

	IF (@vNumParte IS NULL OR @vNumParte = '')
	BEGIN
		-- OBTENER número de parte
		SET @vNumParte = (SELECT TOP 1 NumParte FROM NumerosParte 
			WHERE UPPER(@pNumParte) LIKE CONCAT('%', CONCAT(UPPER(NumParte),'%')))
	END
	--ELSE
	--BEGIN		
		--SELECT 'NOEXISTE' Resultado
	--END
	SELECT @vNumParte AS NumParte

  END TRY
  BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spNumerosParteObtener'
  	SET @vIncidente = 'Error al obtener un Números de parte.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_022_Crea_spNumerosParteObtener.sql', 'Script que valida si el Número de Parte existen la tabla NumerosParte', GETDATE())
GO