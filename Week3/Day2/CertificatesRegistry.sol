pragma solidity ^0.4.18;

import './modifiers/Owned.sol';

contract CertificateRegistry is Owned {
    mapping(bytes32 => bool) public certificates;

    function addCertificate(bytes32 certificate) public onlyOwner {
        certificates[certificate] = true;
    }
}