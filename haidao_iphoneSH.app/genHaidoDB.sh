
if [ $# -ne 1 ];then
	echo "usage: $0 [all|tableName]"
	exit 0
fi

TABLES=table_list.txt

if ! [ -e $TABLES ];then
	echo "ERROR: not found: $TABLES"
	exit 1
fi

work_dir=`pwd`
#xml_dir=$work_dir/../导出工具表/xmls
xml_dir=.
xml2db=nodeXml2sqlite3.rb
talk2db=talk2sqlite3.rb
db=$xml_dir/haidao.db

while read line
do
  if [ $1 == $line ] || [ $1 == "all" ];then
	
	echo "***** begin $line to haidao.db *****"	
	if [ $line == "talk" ];then
		DEBUG=1 ruby $talk2db $db $xml_dir/$line.xml
	else
		DEBUG=1 ruby $xml2db $db $xml_dir/$line.xml
	fi
	echo "***** end $line to haidao.db *****"	
	echo
  fi
done < $TABLES 
