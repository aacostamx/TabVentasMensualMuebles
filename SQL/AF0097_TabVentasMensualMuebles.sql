-- =============================================
-- Procedimiento AF0097_TabVentasMensualMuebles
-- =============================================
use Carteras
go
if object_id ('Carteras.dbo.AF0097_TabVentasMensualMuebles ','P') is not null 
drop procedure  AF0097_TabVentasMensualMuebles
go
create procedure AF0097_TabVentasMensualMuebles
as

-- =============================================
-- Autor: Antonio Acosta Murillo
-- Fecha: 14 Enero 2014
-- Descripción General: Se cambio la logica para hacerlo mas rapido.
-- =============================================
-- =============================================
-- Autor: Antonio Acosta Murillo
-- Fecha: 13 Enero 2014
-- Descripción General: Se modificó el procedimiento porque la MaeCartera en Operacion no tiene el campo vencido (se calculara con la funcion fngeVencidoMuebles).
-- =============================================
-- =============================================
-- Autor: Antonio Acosta Murillo
-- Fecha: 06 Enero 2014
-- Descripción General: Genera tabulado de la Cartera de Muebles por mes de venta.
-- =============================================
begin

--****************************************************************************************
--****************************************************************************************				
--Se declara la variable fecha donde se almacenara la fecha del catFechas
declare @Fecha as smalldatetime
select @Fecha=Fecha from CatFechas

--Se calcula el el saldo, vencido y numero de operaciones de la Cartera de Muebles agrupada por fecha y tipoproducto
if exists (select * from sysobjects where name = 'tmp_CarxTipoProducto00') drop table tmp_CarxTipoProducto00
select month(FechaVenta)Mes,year(FechaVenta)Anio,TipoProducto,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,
sum(cast(ltrim(rtrim(substring(dbo.fngeVencidoMuebles(@fecha,FechaVenta,ImporteVenta,InteresSobreCompra,PlazoVenta,AbonoMensual,SaldoALAFecha,Enganche),34,11))) as float))Vencido,
count(*)NumCuentas
into tmp_CarxTipoProducto00
from MaeCarteraMuebles
group by year(FechaVenta),month(FechaVenta),TipoProducto
order by anio desc, mes desc, TipoProducto

--Se calcula el saldo en miles y el vencido en miles (son necesarios para el reporte rpt en crystal reports)
if exists (select * from sysobjects where name = 'tmp_CarxTipoProducto') drop table tmp_CarxTipoProducto
select Mes,Anio,TipoProducto,Saldo,round(cast(Saldo as float)/1000,0)SaldoenMiles,Vencido,round(cast(Vencido as float)/1000,0)VencidoenMiles,NumCuentas
into tmp_CarxTipoProducto
from tmp_CarxTipoProducto00

--Se crea el campo mes con el formato MmmAAAA ejemplo: Ene2013 
if exists(select * from sysobjects where name = 'tmp_CarxTipoProductoFINAL') drop table tmp_CarxTipoProductoFINAL
select case
    when Mes=1 then 'Ene'+ cast(anio as char(4))
	when Mes=2 then 'Feb'+ cast(anio as char(4)) 
	when Mes=3 then 'Mar'+ cast(anio as char(4)) 
	when Mes=4 then 'Abr'+ cast(anio as char(4)) 
	when Mes=5 then 'May'+ cast(anio as char(4)) 
	when Mes=6 then 'Jun'+ cast(anio as char(4)) 
	when Mes=7 then 'Jul'+ cast(anio as char(4)) 
	when Mes=8 then 'Ago'+ cast(anio as char(4)) 
	when Mes=9 then 'Sep'+ cast(anio as char(4)) 
	when Mes=10 then 'Oct'+ cast(anio as char(4)) 
	when Mes=11 then 'Nov'+ cast(anio as char(4)) 
	when Mes=12 then 'Dic'+ cast(anio as char(4))
end Fecha,*
	into tmp_CarxTipoProductoFINAL
	from tmp_CarxTipoProducto
	
--****************************************************************************************
--Calcula el total del vencido y saldo de las ventas mensuales de muebles 
--****************************************************************************************
if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal') drop table tmp_VtasMensualMueblesFinal
select Fecha,Mes,Anio,sum(Saldo)Saldo,sum(SaldoEnMiles)SaldoEnMiles,sum(Vencido)Vencido,sum(VencidoenMiles)VencidoenMiles,sum(NumCuentas)NumCuentas
into tmp_VtasMensualMueblesFinal
from tmp_CarxTipoProductoFINAL
group by Fecha,Mes,Anio
order by anio desc, mes desc

--****************************************************************************************
--Calcula el total del vencido y saldo de las ventas mensuales de muebles - Coppel
--****************************************************************************************
if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal_Coppel') drop table tmp_VtasMensualMueblesFinal_Coppel
select Fecha,Mes,Anio,sum(Saldo)Saldo,sum(SaldoEnMiles)SaldoEnMiles,sum(Vencido)Vencido,sum(VencidoenMiles)VencidoenMiles,sum(NumCuentas)NumCuentas
into tmp_VtasMensualMueblesFinal_Coppel
from tmp_CarxTipoProductoFINAL
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=0)
group by Fecha,Mes,Anio
order by anio desc, mes desc

--****************************************************************************************
--Calcula el total del vencido y saldo de las ventas mensuales de muebles - Negocios Afiliados
--***************************************************************************************
if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal_NegAfiliado') drop table tmp_VtasMensualMueblesFinal_NegAfiliado
select Fecha,Mes,Anio,sum(Saldo)Saldo,sum(SaldoEnMiles)SaldoEnMiles,sum(Vencido)Vencido,sum(VencidoenMiles)VencidoenMiles,sum(NumCuentas)NumCuentas
into tmp_VtasMensualMueblesFinal_NegAfiliado
from tmp_CarxTipoProductoFINAL
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=1)
group by Fecha,Mes,Anio
order by anio desc, mes desc

--select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas 
--from tmp_VtasMensualMueblesFinal

--select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas 
--from tmp_VtasMensualMueblesFinal_Coppel

--select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas 
--from tmp_VtasMensualMueblesFinal_NegAfiliado

end