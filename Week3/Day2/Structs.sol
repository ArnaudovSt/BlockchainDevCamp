pragma solidity ^0.4.18;

import './modifiers/Owned.sol';

contract Structs is Owned {
    struct Credentials {
        bytes16 firstName;
        bytes16 lastName;
        bytes32 email;
    }

    mapping (address=>Credentials) credentials;

    function getCredentials(address _user) onlyOwner public view returns(bytes16 firstName, bytes16 lastName, bytes32 email) {
        require(_user != address(0));
        firstName = credentials[_user].firstName;
        lastName = credentials[_user].lastName; 
        email = credentials[_user].email;
    }

    function postCredentials(bytes16 _firstName, bytes16 _lastName, bytes32 _email) public {
        credentials[msg.sender] = Credentials(_firstName, _lastName, _email);
    }
}