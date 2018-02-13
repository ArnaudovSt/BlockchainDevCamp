pragma solidity ^0.4.18;

contract SimpleStorage {
    uint256 public stateVariable;

    function setVariable(uint256 _value) public {
        stateVariable = _value;
    }
}