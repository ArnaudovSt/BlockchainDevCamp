pragma solidity ^0.4.18;

contract Incrementor {
    uint256 public stateVariable;

    function increment(uint256 _incrementBy) public {
        uint256 result = stateVariable + _incrementBy;
        assert(result >= stateVariable);
        stateVariable = result;
    }
}