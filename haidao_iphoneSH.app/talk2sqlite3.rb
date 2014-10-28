
require 'rexml/document'
require 'sqlite3'

DEBUG = ENV["DEBUG"] || false 

if ARGV.count != 2 
  puts "usage: ruby xml2sqlite3.rb db_file xml_file"
  exit 0
end
dbFile = ARGV[0]
file = ARGV[1]

def create_attr_table(tbl, fieldsElements)
  sql = "DROP TABLE IF EXISTS #{tbl}"
  @db.execute(sql)
  puts "delete exist table: #{tbl}, ret = #{@db.total_changes}" if DEBUG

  #create table **************************************************************************
  # 检查所有的elements，取attributes个数最多的作为create table的参考
  max_count = 0
  max_idx = 0
  cur_idx = 0
  fieldsElements.each { |e|
     if e.attributes.length > max_count
       max_count =  e.attributes.length
       max_idx = cur_idx
       puts "max_count = #{max_count} max_idx = #{max_idx}" if DEBUG
     end
     cur_idx += 1
  }
  max_idx += 1 #查rexml文档elements下标访问是从1开始, 所以最后要加1
  puts "max_count = #{max_count}" if DEBUG
  puts "max fields count: #{fieldsElements[max_idx].attributes.length}" if DEBUG

  @orig_fields.clear
  conflict_field = "" #保存和sqliteKeywords冲突的字段名
  sql = "CREATE TABLE #{tbl} ("
  fields_create = Array.new #保存建表用的所有字段定义
  fieldsElements[max_idx].attributes.each { |k,v|
    puts "k = #{k}  v = #{v}" if DEBUG
    @orig_fields.push(k)
=begin
    if @sqliteKeywords.include?(k.upcase)
      conflict_field += k + '--' + "#{k}_yl" + ' '
      k = "#{k}_yl"
    end
=end
    fields_create.push("#{k} text")
  }
  sql += fields_create.join(',') + ')'
  puts "create table sql = #{sql}" #if DEBUG
  @db.execute(sql)

  if conflict_field != ""  #如果有冲突的字段名保存到文件
    system "echo #{tbl}.xml: #{conflict_field} >> #{@conflict_field_file}"
    conflict_field = ""
  end
end #end for create_attr_table

def insert_attr_data(tbl, itemsElements)
  puts "record.count = #{itemsElements.count}" if DEBUG
  itemsElements.each { |elem|
    sql = "INSERT INTO #{tbl} VALUES ("
    values = Array.new
    @orig_fields.each { |k|
      v = elem.attributes[k]
      puts "\tattr: #{k} = #{v}" if DEBUG
      #vv = v.to_str
      #vv.gsub!(/'/, '"') #use " replace ', avoid sql error
	  vv = nil 
	  #vv = "\"#{v}\"" if v
	  vv = v.gsub(/'/, "''") if v 
	  puts "vv = #{vv}"
	  #values.push( "#{v || ""}")
	  values.push("'#{vv}'")
    }
    sql += values.join(',') + ')'
    puts "insert_attr_data: sql: #{sql}" if DEBUG

    @db.execute(sql)
    #break if DEBUG
  }
  @orig_fields.clear
end #end of insert_attr_data

def create_table_by_foreign_key(tbl, fieldsElements, foreign_key_def)
  sql = "DROP TABLE IF EXISTS #{tbl}"
  @db.execute(sql)
  puts "delete exist table: #{tbl}, ret = #{@db.total_changes}" if DEBUG

  #create table **************************************************************************
  # 检查所有的elements，取attributes个数最多的作为create table的参考
  max_count = 0
  max_idx = 0
  cur_idx = 0
  fieldsElements.each { |e|
     elem = e.last
     if elem.attributes.length > max_count
       max_count = elem.attributes.length
       max_idx = cur_idx
       puts "max_count = #{max_count} max_idx = #{max_idx}" if DEBUG
     end
     cur_idx += 1
  }
  puts "max_count = #{max_count}" if DEBUG
  puts "max fields count: #{fieldsElements[max_idx].last.attributes.length}" if DEBUG

  @orig_fields.clear
  conflict_field = "" #保存和sqliteKeywords冲突的字段名
  sql = "CREATE TABLE #{tbl} ("
  fields_create = Array.new #保存建表用的所有字段定义
  fields_create.push(foreign_key_def) #eg. 'talk_id text'
  fieldsElements[max_idx].last.attributes.each { |k,v|
    puts "k = #{k}  v = #{v}" if DEBUG
    @orig_fields.push(k)
=begin
    if @sqliteKeywords.include?(k.upcase)
      conflict_field += k + '--' + "#{k}_yl" + ' '
      k = "#{k}_yl"
    end
=end
    fields_create.push("#{k} text")
  }
  sql += fields_create.join(',') + ')'
  puts "create table sql = #{sql}" #if DEBUG
  @db.execute(sql)
=begin
  if conflict_field != ""  #如果有冲突的字段名保存到文件
    system "echo #{tbl}.xml: #{conflict_field} >> #{@conflict_field_file}"
    conflict_field = ""
  end
=end
end #end for create_table_by_foreign_key 

def insert_data_for_foreign_key(tbl, itemsArray)
  puts "record.count = #{itemsArray.count}" if DEBUG
  itemsArray.each { |elem|
    sql = "INSERT INTO #{tbl} VALUES ("
    values = Array.new
    values.push(elem.first) # add talk_id
    values.push(elem[1]) if tbl == "option" # add dialog_id
    @orig_fields.each { |k|
      v = elem.last.attributes[k]
      puts "\tattr: #{k} = #{v}" if DEBUG
	  vv = nil
	  vv = v.gsub(/'/, "''") if v
	  values.push("'#{vv}'")
    }
    sql += values.join(',') + ')'
    puts "insert_data_for_foreign_key: sql: #{sql}" if DEBUG

    @db.execute(sql)
    #break if DEBUG
  }
  @orig_fields.clear
end #end of insert_data_for_foreign_key 

#main progress *********************************************************************
beginSec = Time.now.to_i

#记录每个表的所有字段名，后面插入数据时，用于检测是否缺少某个字段
@orig_fields = Array.new
@conflict_field_file = "field_name_conflict_tables.txt"

#load xml
xmlFd = File.open(file)
doc = REXML::Document.new(xmlFd)

#open db, drop table
@db = SQLite3::Database.new(dbFile)

#<talk id="19" tips="王国学院_主线_1006_打败部队_03_已接">
#<dialog id="1" msg="城中情况如何了？" type="0" leftHeaderId="" rightHeaderId="" dir="" talkName="">
#<option type="1" color="" data="." text="全部交给我吧！"/>
#</dialog>
#</talk>

#create talk
tbl = 'talk'
talkElements = doc.root.elements
puts "#{tbl} count = #{talkElements.count}" #if DEBUG
create_attr_table(tbl, talkElements)
insert_attr_data(tbl, talkElements)


dlgElements = Array.new
optElements = Array.new
talkElements.each { |talk|
   #puts "dialog.count = #{talk.elements.count}"
   talk.elements.each { |dialog|
     talk_dlg = [talk.attributes['id'], dialog] 
     dlgElements.push(talk_dlg)

     dialog.elements.each { |opt|
       dlg_opt =  [talk.attributes['id'], dialog.attributes['id'], opt]
       optElements.push(dlg_opt)
     }
   }
}

puts "dialogs = #{dlgElements.length}"
tbl = 'dialog'
create_table_by_foreign_key(tbl, dlgElements, "talk_id text")
insert_data_for_foreign_key(tbl, dlgElements) 

puts "opts = #{optElements.length}"
tbl = 'option'
create_table_by_foreign_key(tbl, optElements, "talk_id text,dialog_id text")
insert_data_for_foreign_key(tbl, optElements) 


#close xml file and sqlite
xmlFd.close
@db.close

#count and show process time
endSec = Time.now.to_i
puts "total use: #{endSec - beginSec} seconds"

