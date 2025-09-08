import { useState, useEffect, useRef, Key } from 'react'
import { z } from 'zod'
import { Cascade } from '@/dto'
import { Member, MemberOptions } from '@/enums'
import { User, UserGroup, Department } from '@/models'
import { findInTree, getAssetsUrl } from '@/utils'
import {
  LoadingOutlined,
  UserOutlined,
  ClusterOutlined,
  TeamOutlined,
} from '@ant-design/icons'
import {
  Modal,
  Col,
  Row,
  Card,
  Segmented,
  Spin,
  Table,
  Tag,
  Avatar,
  Badge,
  Form,
  Select,
  Input,
  Button,
  Cascader,
  Flex,
  Tree,
  TreeProps,
  TablePaginationConfig,
} from 'antd'
import _ from 'lodash'
import modalSelectMembersApi, {
  GetResult,
} from '@/api/common/modalSelectMembersApi'

interface ModalSelectMembersProps {
  title: string
  members: string[]
  onClose: () => void
  onChange: (
    selectedUsers: User[],
    selectedDepartments: Department[],
    selectedGroups: UserGroup[]
  ) => void
}

const ModalSelectMembers: React.FC<ModalSelectMembersProps> = ({
  title,
  members,
  onClose,
  onChange,
}) => {
  const [loading, setLoading] = useState(false)
  const [segment, setSegment] = useState(members[0])
  const [selectedUserIds, setSelectedUserIds] = useState<number[]>([])
  const [selectedUsers, setSelectedUsers] = useState<User[]>([])
  const rightRef = useRef<HTMLDivElement>(null)
  const [request, setRequest] = useState<GetResult>({
    departments: [],
    groups: [],
    count: 0,
    users: [],
    pageSize: 10,
    userLevelCount: 0,
    userLevelName1: '',
    userLevelName2: '',
    userLevelName3: '',
    userLevelName4: '',
    userLevelName5: '',
    userLevelName6: '',
    userLevelName7: '',
    userLevelName8: '',
    userLevelName9: '',
  })
  const [formInline, setFormInline] = useState<{
    departmentIds: number[][]
    groupId: number
    keyword: string
    page: number
    pageSize: number
  }>({
    departmentIds: [],
    groupId: -1,
    keyword: '',
    page: 1,
    pageSize: 10,
  })

  const [selectedDepartmentIds, setSelectedDepartmentIds] = useState<number[]>(
    []
  )
  const [selectedDepartments, setSelectedDepartments] = useState<Department[]>(
    []
  )

  const [filterGroups, setFilterGroups] = useState<string>('')
  const [selectedGroupIds, setSelectedGroupIds] = useState<number[]>([])
  const [selectedGroups, setSelectedGroups] = useState<UserGroup[]>([])

  const apiGet = async () => {
    const res = await modalSelectMembersApi.get()
    if (res) {
      setRequest({ ...res })
    }
  }

  const apiGetDepartments = async (keyword: string) => {
    const res = await modalSelectMembersApi.departments({
      keyword,
    })
    if (res) {
      setRequest({ ...request, departments: res.departments })
    }
  }

  useEffect(() => {
    apiGet()
  }, [])

  const handleOk = () => {
    onChange(selectedUsers, selectedDepartments, selectedGroups)
  }

  const handleCancel = () => {
    onClose()
  }

  const handleUserSearch = async (page: number) => {
    const departmentIds: number[] = []
    for (const departmentIdList of formInline.departmentIds) {
      if (Array.isArray(departmentIdList)) {
        departmentIds.push(departmentIdList[departmentIdList.length - 1])
      } else {
        departmentIds.push(departmentIdList)
      }
    }

    setLoading(true)
    const res = await modalSelectMembersApi.users({
      departmentIds,
      groupId: formInline.groupId,
      keyword: formInline.keyword,
      page,
      pageSize: request.pageSize,
    })
    if (res) {
      setRequest({ ...request, users: res.users, count: res.count })
      setFormInline({ ...formInline, page })
    }
    setLoading(false)
  }

  const handleUserTableChange = async (pagination: TablePaginationConfig) => {
    await handleUserSearch(pagination.current ?? 1)
  }

  const handleUserSelectionChange = (selectedRowKeys: Key[]) => {
    setSelectedUserIds(selectedRowKeys.map((key) => Number(key)))
    let users = selectedRowKeys.map((userId) =>
      request.users.find((user) => user.id === userId)
    )
    users = users.filter(
      (user) =>
        user &&
        !selectedUsers.some((selectedUser) => selectedUser.id === user.id)
    )
    setSelectedUsers([
      ...selectedUsers,
      ...users.filter((user): user is User => user !== undefined),
    ])
  }

  const handleUserClose = (user: User) => {
    setSelectedUserIds(selectedUserIds.filter((id) => id !== user.id))
    setSelectedUsers(
      selectedUsers.filter((selectedUser) => selectedUser.id !== user.id)
    )
  }

  const handleDepartmentSelectionChange: TreeProps['onCheck'] = (
    checkedKeys
  ) => {
    if (
      !z.object({ checked: z.array(z.string()) }).safeParse(checkedKeys).success
    ) {
      return
    }
    checkedKeys = checkedKeys as { checked: Key[]; halfChecked: Key[] }
    const selectedDepartmentIds = checkedKeys.checked.map((departmentId) =>
      parseInt(departmentId.toString())
    )
    setSelectedDepartmentIds(selectedDepartmentIds)
    let departments = selectedDepartmentIds.map((departmentId) => {
      const node = findInTree(
        request.departments,
        (item) => item.value === departmentId
      )
      return node?.department as Department
    })
    departments = departments.filter(
      (department) =>
        department &&
        !selectedDepartments.some(
          (selectedDepartment) => selectedDepartment.id === department.id
        )
    )

    setSelectedDepartments([
      ...selectedDepartments,
      ...departments.filter(
        (department): department is Department => department !== undefined
      ),
    ])
  }

  const handleDepartmentClose = (department: Department) => {
    setSelectedDepartmentIds(
      selectedDepartmentIds.filter((id) => id !== department.id)
    )
    setSelectedDepartments(
      selectedDepartments.filter(
        (selectedDepartment) => selectedDepartment.id !== department.id
      )
    )
  }

  const renderTreeNodes = (data: Cascade<number>[]) =>
    data.map((item, index) => (
      <Tree.TreeNode
        key={item.value + '-' + index}
        title={<div>{item.label}</div>}
      >
        {item.children && renderTreeNodes(item.children)}
      </Tree.TreeNode>
    ))

  const handleGroupSelectionChange = (selectedRowKeys: Key[]) => {
    setSelectedGroupIds(selectedRowKeys.map((key) => Number(key)))
    let groups = selectedRowKeys.map((groupId) =>
      request.groups.find((group: UserGroup) => group.id === groupId)
    )
    groups = groups.filter(
      (group) =>
        group &&
        !selectedGroups.some((selectedGroup) => selectedGroup.id === group.id)
    )
    setSelectedGroups([
      ...selectedGroups,
      ...groups.filter((group): group is UserGroup => group !== undefined),
    ])
  }

  const handleGroupClose = (group: UserGroup) => {
    setSelectedGroupIds(selectedGroupIds.filter((id) => id !== group.id))
    setSelectedGroups(
      selectedGroups.filter((selectedGroup) => selectedGroup.id !== group.id)
    )
  }

  return (
    <Modal
      title={title}
      open={true}
      onOk={handleOk}
      onCancel={handleCancel}
      okText='确 认'
      cancelText='取 消'
      style={{
        top: 20,
        marginLeft: 20,
      }}
      width={`${window.innerWidth - 40}px`}
      styles={{
        body: {
          height: `${window.innerHeight - 160}px`,
          overflowY: 'auto',
          overflowX: 'hidden',
        },
      }}
    >
      <Spin
        spinning={loading}
        indicator={<LoadingOutlined spin />}
        size='large'
      >
        <Row gutter={[16, 16]}>
          <Col span={16}>
            <Card
              size='small'
              style={{
                width: '100%',
              }}
            >
              {members.length > 1 && (
                <Segmented
                  options={MemberOptions}
                  value={segment}
                  onChange={setSegment}
                  block
                />
              )}
              <div
                style={{
                  display: segment === Member.Users ? 'block' : 'none',
                  marginTop: '10px',
                }}
              >
                <Flex vertical={true} gap='middle'>
                  <Form layout='inline'>
                    <Form.Item label='部门'>
                      <Cascader
                        options={request.departments}
                        value={formInline.departmentIds}
                        onChange={(value) => {
                          setFormInline({ ...formInline, departmentIds: value })
                        }}
                        placeholder='请选择部门'
                        showSearch
                        multiple
                        style={{ width: 200 }}
                      />
                    </Form.Item>
                    <Form.Item label='用户组'>
                      <Select
                        value={formInline.groupId}
                        onChange={(value) =>
                          setFormInline({ ...formInline, groupId: value })
                        }
                        placeholder='用户组'
                        style={{ width: 150 }}
                      >
                        <Select.Option value={-1}>全部用户组</Select.Option>
                        {request.groups.map((group, index) => (
                          <Select.Option key={index} value={group.id}>
                            {group.groupName}
                          </Select.Option>
                        ))}
                      </Select>
                    </Form.Item>
                    <Form.Item label='关键词'>
                      <Input
                        value={formInline.keyword}
                        onChange={(e) =>
                          setFormInline({
                            ...formInline,
                            keyword: e.target.value,
                          })
                        }
                        placeholder='搜索'
                      />
                    </Form.Item>
                    <Form.Item>
                      <Button
                        type='primary'
                        onClick={() => handleUserSearch(1)}
                      >
                        查询
                      </Button>
                    </Form.Item>
                  </Form>

                  <Table
                    dataSource={request.users}
                    rowKey='id'
                    pagination={{
                      current: formInline.page,
                      pageSize: request.pageSize,
                    }}
                    onChange={handleUserTableChange}
                    rowSelection={{
                      selectedRowKeys: selectedUserIds,
                      onChange: handleUserSelectionChange,
                    }}
                    onRow={(record: User) => ({
                      onClick: () =>
                        handleUserSelectionChange(
                          selectedUserIds.includes(record.id)
                            ? selectedUserIds.filter((id) => id !== record.id)
                            : [...selectedUserIds, record.id]
                        ),
                    })}
                  >
                    <Table.Column
                      title='头像'
                      align='center'
                      width={120}
                      render={(_, record) => {
                        const userLevelName = record.manager
                          ? request['userLevelName' + record.level]
                          : null
                        if (userLevelName) {
                          return (
                            <Badge count={userLevelName as string}>
                              <Avatar
                                size={40}
                                src={
                                  record.avatarUrl ||
                                  getAssetsUrl('images/default_avatar.png')
                                }
                              />
                            </Badge>
                          )
                        }
                        return (
                          <Avatar
                            size={40}
                            src={
                              record.avatarUrl ||
                              getAssetsUrl('images/default_avatar.png')
                            }
                          />
                        )
                      }}
                    />
                    <Table.Column title='部门' dataIndex='departmentName' />
                    <Table.Column title='姓名' dataIndex='displayName' />
                    <Table.Column
                      title='账号'
                      render={(_, record) => (
                        <>
                          <a>{record.userName}</a>
                          {!record.checked && <Tag color='red'>待审核</Tag>}
                          {record.locked && <Tag color='red'>已锁定</Tag>}
                        </>
                      )}
                    />
                    <Table.Column
                      title='用户组'
                      width={150}
                      render={(_, record) =>
                        record.groups.map((group: UserGroup, index: number) => (
                          <Tag
                            key={index}
                            style={{ marginRight: 5, marginBottom: 5 }}
                          >
                            {group.groupName}
                          </Tag>
                        ))
                      }
                    />
                  </Table>
                </Flex>
              </div>
              <div
                style={{
                  display: segment === Member.Departments ? 'block' : 'none',
                  marginTop: '10px',
                }}
              >
                <Flex vertical={true} gap='middle'>
                  <Form layout='inline'>
                    <Form.Item label='关键词'>
                      <Input
                        onChange={async (e) => {
                          const debouncedSearch = _.debounce(async (value) => {
                            await apiGetDepartments(value)
                          }, 1000)
                          debouncedSearch(e.target.value)
                        }}
                        placeholder='搜索'
                      />
                    </Form.Item>
                  </Form>
                  <Tree
                    checkable
                    blockNode
                    checkStrictly={true}
                    onCheck={handleDepartmentSelectionChange}
                  >
                    {renderTreeNodes(request.departments)}
                  </Tree>
                </Flex>
              </div>
              <div
                style={{
                  display: segment === Member.Groups ? 'block' : 'none',
                  marginTop: '10px',
                }}
              >
                <Flex vertical={true} gap='middle'>
                  <Form layout='inline'>
                    <Form.Item label='关键词'>
                      <Input
                        value={filterGroups}
                        onChange={(e) => setFilterGroups(e.target.value)}
                        placeholder='搜索'
                      />
                    </Form.Item>
                  </Form>
                  <Table
                    dataSource={request.groups.filter((group) =>
                      group.groupName.includes(filterGroups)
                    )}
                    rowKey='id'
                    rowSelection={{
                      selectedRowKeys: selectedGroupIds,
                      onChange: handleGroupSelectionChange,
                    }}
                    onRow={(record) => ({
                      onClick: () =>
                        handleGroupSelectionChange(
                          selectedGroupIds.includes(record.id)
                            ? selectedGroupIds.filter((id) => id !== record.id)
                            : [...selectedGroupIds, record.id]
                        ),
                    })}
                  >
                    <Table.Column
                      title='用户组名称'
                      render={(_, record) => <>{record.groupName}</>}
                    />
                    <Table.Column title='用户组说明' dataIndex='description' />
                  </Table>
                </Flex>
              </div>
            </Card>
          </Col>
          <Col span={8} ref={rightRef}>
            <Card
              size='small'
              style={{
                width:
                  rightRef && rightRef.current
                    ? rightRef.current.scrollWidth - 30 + 'px'
                    : '100%',
                position: 'fixed',
                top: '74px',
              }}
            >
              {selectedUsers && selectedUsers.length > 0 && (
                <div>
                  <h4>已选择以下人员：</h4>
                  {selectedUsers.map((user, index) => {
                    return (
                      <Tag
                        key={index}
                        closable
                        onClose={() => handleUserClose(user)}
                        style={{ marginRight: 5, marginBottom: 5 }}
                        icon={<UserOutlined style={{ fontSize: 14 }} />}
                      >
                        {user.displayName}
                        {user.departmentName && `（${user.departmentName}）`}
                      </Tag>
                    )
                  })}
                </div>
              )}
              {selectedDepartments && selectedDepartments.length > 0 && (
                <div>
                  <h4>已选择以下部门：</h4>
                  {selectedDepartments.map((department, index) => {
                    return (
                      <Tag
                        key={index}
                        closable
                        onClose={() => handleDepartmentClose(department)}
                        style={{ marginRight: 5, marginBottom: 5 }}
                        icon={<ClusterOutlined style={{ fontSize: 14 }} />}
                      >
                        {department.fullName}
                      </Tag>
                    )
                  })}
                </div>
              )}
              {selectedGroups && selectedGroups.length > 0 && (
                <div>
                  <h4>已选择以下用户组：</h4>
                  {selectedGroups.map((group, index) => {
                    return (
                      <Tag
                        key={index}
                        closable
                        onClose={() => handleGroupClose(group)}
                        style={{ marginRight: 5, marginBottom: 5 }}
                        icon={<TeamOutlined style={{ fontSize: 14 }} />}
                      >
                        {group.groupName}
                      </Tag>
                    )
                  })}
                </div>
              )}
            </Card>
          </Col>
        </Row>
      </Spin>
    </Modal>
  )
}

export default ModalSelectMembers
