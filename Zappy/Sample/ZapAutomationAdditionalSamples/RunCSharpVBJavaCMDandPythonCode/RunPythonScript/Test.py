import os
home = os.path.expanduser('~')
location = os.path.join(home, 'Documents')
pathhgh =os.path.join(location ,"ZappySamples", "file.txt")
file1 = open(pathhgh,"w")
file1.write("Hello \nWelcome to our zap automation")
file1.close()