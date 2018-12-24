state("vampire") { int isLoading : "client.dll", 0x005F9C28; } 
isLoading { return current.isLoading !=0; }