delete from d_item
DBCC CHECKIDENT (d_item, RESEED, 1)