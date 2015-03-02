rm result.txt
for /L %%n in (0,1,8) do SFM.exe -o 0 -a %%n -f "E:\Submodular\DataInteger\UndirectedCut\32_0" -r "result.txt"
@echo "au"
wait