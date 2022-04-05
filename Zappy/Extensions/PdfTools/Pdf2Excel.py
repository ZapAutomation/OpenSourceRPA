import camelot
import pandas as pd
import sys

#print(sys.argv[0])
#print(sys.argv[1])
#print(sys.argv[2])
#print(sys.argv[3])
Pdftype = sys.argv[4] # Lattice(Data Based) or Stream(Image Based)
if Pdftype=="lattice":
    tables = camelot.read_pdf(sys.argv[1],pages=sys.argv[3],password=sys.argv[10],flavor=sys.argv[4],suppress_stdout=False,layout_kwargs={},line_overlap=sys.argv[5],char_margin=sys.argv[6],line_margin=sys.argv[7],word_margin=sys.argv[8],boxes_flow=sys.argv[9],detect_vertical=True,all_texts=True)
    tol= len(tables)
    res2 = tables[0].df
    i=1
    while i<tol:
        oput = tables[i].df
        res2 = pd.concat([res2, oput])
        i += 1
    Output_Data=res2
    Output_Data.to_excel(sys.argv[2])
else:
    tables = camelot.read_pdf(sys.argv[1],pages=sys.argv[3],password=sys.argv[10],flavor=sys.argv[4],suppress_stdout=False,layout_kwargs={},table_regions=None,table_areas=None,columns=None,split_text=False,flag_size=False,strip_text='',edge_tol=5,row_tol=1,column_tol=0)
    tol= len(tables)
    res2 = tables[0].df
    i=1
    while i<tol:
        oput = tables[i].df
        res2 = pd.concat([res2, oput])
        i += 1
    Output_Data=res2
    Output_Data.to_excel(sys.argv[2])