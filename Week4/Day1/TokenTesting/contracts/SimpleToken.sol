pragma solidity ^0.4.18;

import '../node_modules/zeppelin-solidity/contracts/token/ERC20/MintableToken.sol';
import '../node_modules/zeppelin-solidity/contracts/token/ERC20/PausableToken.sol';

contract SimpleToken is MintableToken, Pausable {
    string public constant name = "Simple Token";
    string public constant symbol = "ST";
    uint8 public constant decimals = 18;
}