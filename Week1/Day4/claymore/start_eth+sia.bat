timeout 15

setx GPU_FORCE_64BIT_PTR 0
setx GPU_MAX_HEAP_SIZE 100
setx GPU_USE_SYNC_OBJECTS 1
setx GPU_MAX_ALLOC_PERCENT 100
setx GPU_SINGLE_ALLOC_PERCENT 100

EthDcrMiner64.exe -epool eth-eu1.nanopool.org:9999 -ewal 0x46a943569B62D71BdA3E0C83057ba5dA932E61AF/A -epsw x -dpool stratum+tcp://sia-eu1.nanopool.org:7777 -dwal 28590e1669f771870f656fd2fd9e097cad55f517f174750f8a594aa935fa908964e6ffc0e6a9/A -epsw x -dcoin sia -asm 1 -dcri 25 -ethi 8 -allpools 1 -dbg 1 -r 1 -wd 1 -minspeed 1 -gser 2 -tt 69 -ttdcr 75 -ttli 80 -tstop 85