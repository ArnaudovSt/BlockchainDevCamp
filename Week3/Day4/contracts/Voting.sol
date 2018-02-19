pragma solidity ^0.4.18;

contract Voting {
    mapping (bytes32=>uint256) private votes;

    function addCandidate(bytes32 _newCandidate) public {
        require(votes[_newCandidate] == 0);
        votes[_newCandidate] = 1;
    }

    function isValidCandidate(bytes32 _candidateName) public view returns (bool) {
        return votes[_candidateName] >= 1;
    }

    function voteForCandidate(bytes32 _candidateName) public {
        require(votes[_candidateName] >= 1);
        votes[_candidateName]++;        
    }

    function totalVotesFor(bytes32 _candidateName) public view returns (uint256) {
        require(votes[_candidateName] >= 1);

        return votes[_candidateName] - 1;
    }
}