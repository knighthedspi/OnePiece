
require 'rexml/document'
require 'sqlite3'

DEBUG = ENV['DEBUG']

if ARGV.count != 2
  puts "usage: ruby xml2sqlite3.rb db_file xml_file"
  puts "xml_file can in list: warFlag.xml, store.xml, roomconfig.xml, itemconfig.xml"
  exit 0
end
dbFile = ARGV[0]
xmlFile = ARGV[1]

#记录每个表的所有字段名，后面插入数据时，用于检测是否缺少某个字段
@orig_fields = Array.new
#@conflict_field_file = "field_name_conflict_tables.txt"
@conflict_fields = Array.new #保存和sqite 关键字冲突导致建表错误的字段名，以备查询
@sqliteKeywords = ['ORDER',
'INDEX',
'ABORT',
'ACTION',
'ADD',
'FTER',
'ALL',
'ALTER',
'ANALYZE',
'AND',
'AS',
'ASC',
'ATTACH',
'AUTOINCREMENT',
'BEFORE',
'BEGIN',
'BETWEEN',
'BY',
'CASCADE',
'CASE',
'CAST',
'CHECK',
'COLLATE',
'COLUMN',
'COMMIT',
'CONFLICT',
'CONSTRAINT',
'CREATE',
'CROSS',
'CURRENT_DATE',
'CURRENT_TIME',
'CURRENT_TIMESTAMP',
'DATABASE',
'DEFAULT',
'DEFERRABLE',
'DEFERRED',
'DELETE',
#'DESC',
'DETACH',
'DISTINCT',
'DROP',
'EACH',
'ELSE', 'END',
'ESCAPE',
'EXCEPT',
'EXCLUSIVE',
'EXISTS',
'EXPLAIN',
'FAIL',
'FOR',
'FOREIGN',
'FROM',
'FULL',
'GLOB',
'GROUP',
'HAVING',
'IF',
'IGNORE',
'IMMEDIATE',
'IN',
'INDEX',
'INDEXED',
'INITIALLY',
'INNER',
'INSERT',
'INSTEAD',
'INTERSECT',
'INTO',
'IS',
'ISNULL',
'JOIN',
#'KEY',
'LEFT',
'LIKE',
'LIMIT',
'MATCH',
'NATURAL',
'NO',
'NOT',
'NOTNULL',
'NULL',
'OF',
'OFFSET',
'ON',
'OR',
'ORDER',
'OUTER',
'PLAN',
'PRAGMA',
'PRIMARY',
'QUERY',
'RAISE',
'REFERENCES',
'REGEXP',
'REINDEX',
'RELEASE',
'RENAME',
'REPLACE',
'RESTRICT',
'RIGHT',
'ROLLBACK',
'ROW',
'SAVEPOINT',
'SELECT',
'SET',
'TABLE',
'TEMP',
'TEMPORARY',
'THEN',
'TO',
'TRANSACTION',
'TRIGGER',
'UNION',
'UNIQUE',
'UPDATE',
'USING',
'VACUUM',
'VALUES',
'VIEW',
'VIRTUAL',
'WHEN',
'WHERE']

def create_single_node_table(tbl, fieldsElements)
 sql = "DROP TABLE IF EXISTS #{tbl}"
 @db.execute(sql)
 puts "delete exist table: #{tbl}, ret = #{@db.total_changes}" if DEBUG
 @orig_fields.clear

 sql = "CREATE TABLE #{tbl} ("
 arrFields = Array.new 

  # 检查所有的elements，取attributes个数最多的作为create table的参考
  max_count = 0
  max_idx = 0
  cur_idx = 0
  fieldsElements.each { |e|
     if e.elements.count > max_count
       max_count =  e.elements.count
       max_idx = cur_idx
       puts "max_count = #{max_count} max_idx = #{max_idx}" if DEBUG
     end
     cur_idx += 1
  }
  max_idx += 1 #查rexml文档elements下标访问是从1开始, 所以最后要加1
  puts "max_count = #{max_count}" if DEBUG
  puts "max fields count: #{fieldsElements[max_idx].elements.count}" if DEBUG

  fieldsElements[max_idx].elements.each { |fd|
    rfd = /<([^>]+)>/.match(fd.to_s)[1]
    @orig_fields.push(rfd)
    arrFields.push("#{rfd} text")
  } 

 sql += arrFields.join(',') + ")"
 puts "create sql: #{sql}" if DEBUG
 @db.execute(sql)
end #create_single_node_table
def insert_single_node_data(tbl, itemsElements)
  arrFields = Array.new
  puts "item count = #{itemsElements.count}"
  itemsElements.each { |item|
    sql = "INSERT INTO #{tbl} VALUES ("
    @orig_fields.each { |ofd|
      v = item.elements[ofd]
	  vv = 'NULL'
	  vv = "'#{v.text}'" if (v && v.text != "undefined")
      #arrFields.push( "'#{v ? (v.text != "undefined" ? v.text : "") : ""}'")
	  arrFields.push(vv)
      puts "orig_field = #{ofd}" if DEBUG
      puts "cur_field = #{item.elements[ofd]}" if DEBUG
      puts "v = #{arrFields.last}" if DEBUG
    }
 
    sql += arrFields.join(',') + ")"
    puts "sql = #{sql}" if DEBUG
    @db.execute(sql)
    arrFields.clear  #clear for next 
   #break if DEBUG
  }
  @orig_fields.clear
end #end for insert_single_node_data

def create_node_table(tbl, fieldsElements)
 sql = "DROP TABLE IF EXISTS #{tbl}"
 @db.execute(sql)
 puts "delete exist table: #{tbl}, ret = #{@db.total_changes}" if DEBUG
 
 haFields = Hash.new #保存所有出现的字段名
  fieldsElements.each { |room|
    room.elements.each { |e|
		haFields[e.name] = 1
	}
 }
 haFields.each { |k, v| puts "key = #{k}"} if DEBUG
 @orig_fields.clear
 @orig_fields = haFields.keys
=begin
 @orig_fields.clear

 sql = "CREATE TABLE #{tbl} ("
 arrFields = Array.new 

 fieldsElements.each { |fd|
   v = fd.attributes['name'] 
   puts "k = name  v = #{v}" if DEBUG
   @orig_fields.push(v)
    arrFields.push(" #{v} text")
 }
 sql += arrFields.join(',') + ")"
 puts "create sql: #{sql}" if DEBUG
 @db.execute(sql)
=end
  sql = "CREATE TABLE #{tbl} (" + haFields.keys.join(' text,') + ')'
  puts "create table sql = #{sql}" #if DEBUG
  @db.execute(sql)
end #end of create_node_table

def insert_node_data(tbl, itemsElements)
  arrFields = Array.new
  puts "item count = #{itemsElements.count}"
  itemsElements.each { |item|
    sql = "INSERT INTO #{tbl} VALUES ("
    @orig_fields.each { |ofd|
      v = item.elements[ofd]
      #arrFields.push( "'#{v ? (v.text != "undefined" ? v.text : "") : ""}'")
	  vv = 'NULL'
	  vv = "'#{v.text}'" if (v && v.text != "undefined")
	  arrFields.push(vv)
      puts "orig_field = #{ofd}" if DEBUG
      puts "cur_field = #{item.elements[ofd]}" if DEBUG
      puts "v = #{arrFields.last}" if DEBUG
    }
 
    sql += arrFields.join(',') + ")"
    puts "sql = #{sql}" if DEBUG
    @db.execute(sql)
    arrFields.clear  #clear for next 
   #break if DEBUG
  }
  @orig_fields.clear
end #end for insert_node_data

def create_attr_table(tbl, fieldsElements)
  sql = "DROP TABLE IF EXISTS #{tbl}"
  @db.execute(sql)
  puts "delete exist table: #{tbl}, ret = #{@db.total_changes}" if DEBUG

  #create table **************************************************************************
  # 检查所有的elements，取attributes个数最多的作为create table的参考
  haFields = Hash.new #保存所有出现的字段名
  fieldsElements.each { |e|
    e.attributes.each { |k, v|
		#puts "#{k} = #{v}" if DEBUG
		#kk = @sqliteKeywords.include?(k.upcase) ? "#{tbl}_#{k}" : k #如果出现字段名和sqlite关键字冲突，在字段名加表名_避免冲突
		kk = k
		if @sqliteKeywords.include?(k.upcase) #如果出现字段名和sqlite关键字冲突，在字段名加表名_避免冲突
		  kk = "#{tbl}_#{k}"
		  @conflict_fields << kk
	    end	
		haFields[kk] = 1
	}
  }
  haFields.each { |k, v| puts "key = #{k}"} if DEBUG
  @orig_fields.clear
  @orig_fields = haFields.keys

  sql = "CREATE TABLE #{tbl} (" + haFields.keys.join(' text,') + ')'
  puts "create table sql = #{sql}" #if DEBUG
  @db.execute(sql)

end #end for create_attr_table

def insert_attr_data(tbl, itemsElements)
  puts "record.count = #{itemsElements.count}" if DEBUG
  prefix = "#{tbl}_" #冲突字段名的前缀
  itemsElements.each { |elem|
    sql = "INSERT INTO #{tbl} VALUES ("
    values = Array.new
    @orig_fields.each { |k|
	  kk = k.include?(prefix) ? k.sub(prefix){|s|} : k
      v = elem.attributes[kk]
      puts "\tattr: #{kk} = #{v}" if DEBUG
	  vv = 'NULL' 
	  vv = "'#{v.gsub(/'/, "''")}'" if v
	  values.push(vv)
      #values.push( "'#{v || ""}'")
    }
    sql += values.join(',') + ')'
    puts "insert_attr_data: sql: #{sql}" if DEBUG

    @db.execute(sql)
    #break if DEBUG
  }
  @orig_fields.clear
end #end of insert_attr_data



def import_xml(file)  # now for itemconfig.xml, roomconfig.xml
  tbl = file.split('.')[0]

  doc = REXML::Document.new(File.open(file))
  fileName = File.basename(file)

  if fileName == "roomconfig.xml"
    #create table ***********************************************************
    #fieldsElements = doc.root.elements['properties'].elements
    #puts "#{tbl} fields count = #{fieldsElements.count}" if DEBUG
    fieldsElements = doc.root.elements['items'].elements
    puts "#{tbl} fields count = #{fieldsElements.count}" if DEBUG
    create_node_table(tbl, fieldsElements)
 
    #insert data
    itemsElements = doc.root.elements['items'].elements
    insert_node_data(tbl, itemsElements)
    return true
  end


  if fileName == "warFlag.xml"
    fieldsElements = doc.root.elements['warFlags'].elements
    puts "#{tbl} fields count = #{fieldsElements.count}" if DEBUG
    create_single_node_table(tbl, fieldsElements)
    insert_single_node_data(tbl, fieldsElements)
    return true
  end

  if fileName == "store.xml"
    #<goods>
    #<good id="1" itemTemplateID="10001" currencyType="1" cost="99999" needItemTemplate="" stock=""/>
    #</goods>
    #<sellers>
    #<seller id="2" name="测试商店2" freshTime="" goods="3,4,21,22,23,24,25"/>
    #</sellers>

    # create table good 
    tbl = 'good'
    fieldsElements = doc.root.elements['goods'].elements
    puts "#{tbl} fields count = #{fieldsElements.count}" if DEBUG
    create_attr_table(tbl, fieldsElements)
    insert_attr_data(tbl, fieldsElements) 

    #create table seller
    tbl = 'seller'
    fieldsElements = doc.root.elements['sellers'].elements
    puts "#{tbl} fields count = #{fieldsElements.count}" if DEBUG
    create_attr_table(tbl, fieldsElements)
    insert_attr_data(tbl, fieldsElements) 
    return true
  end

  if fileName == 'itemconfig.xml'
	puts "itemconfig.xml xxxx" if DEBUG
	tbl = 'itemconfig'
    fieldsElements = doc.root.elements['items'].elements
    puts "#{tbl} fields count = #{fieldsElements.count}" if DEBUG
    create_attr_table(tbl, fieldsElements)
    insert_attr_data(tbl, fieldsElements) 
	return true
  end

  #other table
  tbl = File.basename(file, '.xml') 
  fieldsElements = doc.root.elements
  puts "#{tbl} fields count = #{fieldsElements.count}" if DEBUG
  create_attr_table(tbl, fieldsElements)
  insert_attr_data(tbl, fieldsElements) 

end #end for import_itemconfig

#main progress ******************************************************************** 
beginSec = Time.now.to_i

@db = SQLite3::Database.new(dbFile)
import_xml(xmlFile)
@db.close

endSec = Time.now.to_i
puts "total use: #{endSec - beginSec} seconds"
@conflict_fields.each {|v|
  puts "conflict field: #{v}"
}
