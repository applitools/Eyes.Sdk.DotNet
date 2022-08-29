import sys
from pathlib import Path
from subprocess import Popen, PIPE
from time import sleep

import psutil
import pytest
import os

platform2dir = {
    "win32": "win",
    "linux": "linux",
    "darwin": "mac"
}
BINARIES_DIR = Path(__file__).parent / "UniversalServerRunner" / "build"
CUR_BINARY = BINARIES_DIR / "UniversalServerRunner"
CUR_BINARY = str(CUR_BINARY) + (".exe" if sys.platform == 'win32' else '')

@pytest.mark.parametrize("reason", ["normal"])
def test_automatic_eyes_universal_termination_on_python_termination(reason):
    net_process = Popen([CUR_BINARY], stdin=PIPE, stdout=PIPE)
    output_line = net_process.stdout.readline()
    assert output_line.startswith(b"Runner Started")
    (eyes_universal,) = psutil.Process(net_process.pid).children()
    assert eyes_universal.is_running()
    termination_type = b"1\n" if reason == "normal" else b"2\n"
    net_process.stdin.write(termination_type)
    net_process.stdin.flush()
    net_process.wait()
    sleep(1)  # eyes-universal receives EOF on stdin and terminates, might take time

    assert not eyes_universal.is_running()


@pytest.mark.parametrize("reason", ["normal"])
def test_parallel_eyes_universal_termination_on_python_termination(reason):
    processes = []
    for _ in range(2):
        net_process = Popen([CUR_BINARY], stdin=PIPE, stdout=PIPE)
        output_line = net_process.stdout.readline()
        assert output_line.startswith(b"Runner Started")
        (eyes_universal,) = psutil.Process(net_process.pid).children()
        assert eyes_universal.is_running()
        processes.append((net_process, eyes_universal))

    for net_process, eyes_universal in processes:
        termination_type = b"1\n" if reason == "normal" else b"2\n"
        net_process.stdin.write(termination_type)
        net_process.stdin.flush()
        net_process.wait()
        sleep(1)  # eyes-universal receives EOF on stdin and terminates, might take time
        assert not eyes_universal.is_running()
