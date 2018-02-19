pragma solidity ^0.4.18;

import './modifiers/Owned.sol';

contract DocumentRegistry is Owned {
    mapping (bytes32=>uint256) documents;

    function add(bytes32 keccak) public onlyOwner returns (uint256 dateAdded) {
        documents[keccak] = now;
        return now;
    }

    function verify(bytes32 keccak) public view returns (uint256 dateAdded) {
        return documents[keccak];
    }
}