pragma solidity ^0.4.18;

import './modifiers/Owned.sol';

contract ArrayOfFacts is Owned {
    bytes32[] public factChunks;

    function writeFact(bytes32 _factChunk) public onlyOwner returns (uint256 length) {
        length = factChunks.push(_factChunk);
    }

    function getFacts(uint16 _startIndex, uint16 _length) public view returns (bytes32[]) {
        uint24 bound = _startIndex + _length;
        require(factChunks.length >= bound);
        
        bytes32[] memory buffer = new bytes32[](_length);
        for (uint24 i = _startIndex; i < bound; i++) {
            buffer[i - _startIndex] = factChunks[i];
        }
        return buffer;
    }

    function factsLength() public view returns (uint256) {
        return factChunks.length;
    }
}