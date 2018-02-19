pragma solidity ^0.4.18;

contract Counter {
    uint256 public count;
    
    function increment() public {
        count++;
    }
    
    function decrement() public {
        count--;
    }
}