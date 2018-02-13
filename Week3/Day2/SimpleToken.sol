pragma solidity ^0.4.18;

import './common/SafeMath.sol';

contract SimpleToken {
    using SafeMath for uint256;

    string public constant name = "DevCampToken";
    string public constant symbol = "DCT";
    uint8 public constant decimals = 18;
    uint256 public constant  totalSupply = 100000 * (10 ** decimals);

    mapping (address => uint256) public balanceOf;
    mapping (address => mapping (address => uint256)) public allowance;

    event Transfer(address indexed _from, address indexed _to, uint256 _value);
    event Burn(address indexed _from, uint256 _value);

    function SimpleToken() public {
        balanceOf[msg.sender] = totalSupply;
    }

    function transfer(address _to, uint256 _value) public {
        _transfer(msg.sender, _to, _value);
    }

    function transferFrom(address _from, address _to, uint256 _value) public returns (bool success) {
        require(_value <= allowance[_from][msg.sender]);

        allowance[_from][msg.sender] = allowance[_from][msg.sender].sub(_value);

        _transfer(_from, _to, _value);
        return true;
    }

    function approve(address _spender, uint256 _value) public returns (bool success) {
        allowance[msg.sender][_spender] = _value;
        return true;
    }

    function _transfer(address _from, address _to, uint256 _value) internal {
        require(_to != address(0));
        require(balanceOf[_from] >= _value);

        balanceOf[_from] = balanceOf[_from].sub(_value);
        balanceOf[_to] = balanceOf[_to].add(_value);

        Transfer(_from, _to, _value);
    }
}