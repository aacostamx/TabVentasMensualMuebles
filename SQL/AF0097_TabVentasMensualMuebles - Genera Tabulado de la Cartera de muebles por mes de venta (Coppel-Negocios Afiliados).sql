use carteras
go

--Tabla de prueba (sólo contiene información del 2012)
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from MaeCarteraMuebles
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:00:05

--Tabla de prueba (sólo contiene información del 2012) COPPEL
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=0)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:00:06

--Tabla de prueba (sólo contiene información del 2012) Negocios Afiliados
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=1)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:00:01

--****************************************************************************************
--****************************************************************************************
--Noviembre 2013
use carteras
go

--ODS (tabla con todos los años)
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from ODS.Nov2013.dbo.MaeCarteraMuebles
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:00:55

--ODS (tabla con todos los años) COPPEL
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from ODS.Nov2013.dbo.MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=0)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:01:13

--ODS (tabla con todos los años) Negocios Afiliados
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from ODS.Nov2013.dbo.MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=1)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:00:33

--****************************************************************************************
--****************************************************************************************
--Diciembre 2013
use carteras
go

--ODS (tabla con todos los años)
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from ODS.Dic2013.dbo.MaeCarteraMuebles
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:00:56

--ODS (tabla con todos los años) COPPEL
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from ODS.Dic2013.dbo.MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=0)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:01:32

--ODS (tabla con todos los años) Negocios Afiliados
select month(FechaVenta)mes,year(FechaVenta)anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,round(sum(cast(isnull(SaldoALaFecha,0) as float))/1000,0)SaldoenMiles,sum(cast(isnull(Vencido,0) as bigint))Vencido,round(sum(cast(isnull(Vencido,0) as float))/1000,0)VencidoenMiles,count(*)NumCuentas
from ODS.Dic2013.dbo.MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=1)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc
--00:01:04

